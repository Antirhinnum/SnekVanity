using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.Players;
using SnekVanity.Core;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class LayeredEyePatchItem : ModItem, IAmAsymmetricSpecial
{
	public override string Texture => $"Terraria/Images/Item_{ItemID.EyePatch}";

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.EyePatch);
		Item.DefaultToAccessory();
		Item.headSlot = -1;
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		Vector2 indicatorOffset = new Vector2(2f, 2f) * Main.inventoryScale;
		Texture2D indicatorTexture = TextureAssets.Cursors[CursorOverrideID.FavoriteStar].Value;
		Rectangle indicatorFrame = indicatorTexture.Frame();

		Vector2 realDrawPosition = position - (TextureAssets.InventoryBack.Value.Size() / 2f) + indicatorOffset;
		spriteBatch.Draw(indicatorTexture, realDrawPosition + (new Vector2(40f, 40f) * Main.inventoryScale), indicatorFrame, Color.White, 0f, indicatorFrame.Size() / 2f, 0.5f, SpriteEffects.None, 0f);
	}

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
	{
		Rectangle frame = TextureAssets.Item[Type].Frame();
		Vector2 origin = frame.Size() / 2f;
		Vector2 itemDrawPosition = Item.position - Main.screenPosition + origin + new Vector2((Item.width / 2) - origin.X, Item.height - frame.Height);
		Vector2 indicatorOffset = new Vector2(-2f, -2f) * scale;
		Texture2D indicatorTexture = TextureAssets.Cursors[CursorOverrideID.FavoriteStar].Value;
		Rectangle indicatorFrame = indicatorTexture.Frame();
		spriteBatch.Draw(indicatorTexture, itemDrawPosition + indicatorOffset + (frame.Size().RotatedBy(rotation) * 0.45f * Item.scale), indicatorFrame, alphaColor, rotation, indicatorFrame.Size() / 2f, 0.5f, SpriteEffects.None, 0f);
	}
	public override void AddRecipes()
	{
		Recipe toLayeredRecipe = CreateRecipe()
		.AddIngredient(ItemID.EyePatch)
		.AddTile(TileID.Loom)
			.Register();

		Recipe.Create(ItemID.EyePatch)
			.AddIngredient(Type)
			.AddTile(TileID.Loom)
			.Register()
			.SortBefore(toLayeredRecipe);
	}

	public sealed class LayeredEyePatchDrawLayer : PlayerDrawLayer
	{
		public override bool IsHeadLayer => true;

		public override Position GetDefaultPosition()
		{
			return new AfterParent(PlayerDrawLayers.Head);
		}

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			return drawInfo.drawPlayer.TryGetModPlayer(out LayeredAccessoriesPlayer layeredAccessoriesPlayer) && layeredAccessoriesPlayer.wearingLayeredEyePatch;
		}

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			for (int i = drawInfo.DrawDataCache.Count - 1; i >= 0; i--)
			{
				// Draw directly over the eyelids. If no eyelids are drawn, assume that the face and eyes are hidden (and thus, so should be eye patch).
				if (!PlayerDrawHelpers.UsesPlayerTexture(drawInfo.DrawDataCache[i], drawInfo.drawPlayer, PlayerTextureID.EyeBlink))
				{
					continue;
				}

				LayeredAccessoriesPlayer layeredAccessoriesPlayer = drawInfo.drawPlayer.GetModPlayer<LayeredAccessoriesPlayer>();
				Main.instance.LoadArmorHead(ArmorIDs.Head.EyePatch); // Not preloaded by vanilla unless the normal eye patch is worn

				// Copied from PlayerDrawLayers::DrawPlayer_21_Head()
				DrawData data = new(TextureAssets.ArmorHead[ArmorIDs.Head.EyePatch].Value, drawInfo.helmetOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect, drawInfo.drawPlayer.bodyFrame, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect)
				{
					shader = layeredAccessoriesPlayer.cEyePatch
				};
				drawInfo.DrawDataCache.Insert(i + 1, data);
				break;
			}
		}
	}
}