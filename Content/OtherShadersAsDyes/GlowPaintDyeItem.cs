using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.CustomDyes;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.OtherShadersAsDyes;

public sealed class GlowPaintDyeItem : ACustomDyeItem
{
	public override string Texture { get; } = $"Terraria/Images/Item_{ItemID.GlowPaint}";
	public override bool HasAnyEffects { get; } = true;

	public override void ModifyDrawData(ref Texture2D texture, ref Color color, ref int shader, Player associatedPlayer, Rectangle? sourceRectangle = null)
	{
		if (associatedPlayer == null)
		{
			return;
		}

		GlowPaintDyePlayer glowPaintDyePlayer = associatedPlayer.GetModPlayer<GlowPaintDyePlayer>();
		if (glowPaintDyePlayer.fullbrightDrawData == null)
		{
			return;
		}

		for (int i = 0; i < glowPaintDyePlayer.fullbrightDrawData.Count; i++)
		{
			DrawData data = glowPaintDyePlayer.fullbrightDrawData[i];
			if (data.texture.Bounds == texture.Bounds && data.sourceRect == sourceRectangle)
			{
				color = data.color;
				glowPaintDyePlayer.fullbrightDrawData.RemoveAt(i);
				break;
			}
		}
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe()
			.AddIngredient(ItemID.GlowPaint)
			.AddTile(TileID.DyeVat)
			.Register()
			.SortAfterFirstRecipesOf(ItemID.DeepPinkPaint);

		Recipe.Create(ItemID.GlowPaint)
			.AddIngredient(Type)
			.AddTile(TileID.DyeVat)
			.Register()
			.SortBefore(recipe);
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

	/// <summary>
	/// In order for Illuminant Coating to work properly, we need to replace certain player DrawDatas' colors with the colorsthat would've generated if the player was drawn in fullbright.
	/// We can't just set the color to Color.White because that voids transparency
	/// </summary>
	[Autoload(Side = ModSide.Client)]
	private sealed class GlowPaintDyePlayer : ModPlayer
	{
		public List<DrawData> fullbrightDrawData;
		private static bool _fullbright = false;

		public override void Load()
		{
			On_LegacyPlayerRenderer.DrawPlayer += RenderFullbrightPlayer;
		}

		private static void RenderFullbrightPlayer(On_LegacyPlayerRenderer.orig_DrawPlayer orig, LegacyPlayerRenderer self, Terraria.Graphics.Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float scale)
		{
			GlowPaintDyePlayer glowPaintDyePlayer = drawPlayer.GetModPlayer<GlowPaintDyePlayer>();
			_fullbright = true;
			PlayerDrawHelpers.GetPlayerDrawData(drawPlayer, drawPlayer.position, out glowPaintDyePlayer.fullbrightDrawData, out _, out _, rotation, rotationOrigin, shadow: shadow, scale: scale);
			_fullbright = false;

			orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, scale);
		}

		public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
			if (_fullbright)
			{
				fullBright = true;
			}
		}
	}
}