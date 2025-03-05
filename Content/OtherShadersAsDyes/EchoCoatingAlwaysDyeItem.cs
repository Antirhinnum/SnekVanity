using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.CustomDyes;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.OtherShadersAsDyes;

public sealed class EchoCoatingAlwaysDyeItem : ACustomDyeItem
{
	public override string Texture { get; } = $"Terraria/Images/Item_{ItemID.EchoCoating}";
	public override bool HasAnyEffects { get; } = true;

	public override void ModifyDrawData(ref Texture2D texture, ref Color color, ref int shader, Player associatedPlayer, Rectangle? sourceRectangle = null)
	{
		color = Color.Transparent;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe()
			.AddIngredient<EchoCoatingDyeItem>()
			.AddTile(TileID.DyeVat)
			.Register()
			.SortAfterFirstRecipesOf(ModContent.ItemType<EchoCoatingDyeItem>());

		Recipe.Create(ModContent.ItemType<EchoCoatingDyeItem>())
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
		spriteBatch.Draw(indicatorTexture, realDrawPosition + (new Vector2(40f, 40f) * Main.inventoryScale), indicatorFrame, Color.Lerp(Color.White, Color.Blue, 0.5f), 0f, indicatorFrame.Size() / 2f, 0.5f, SpriteEffects.None, 0f);
	}

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
	{
		Rectangle frame = TextureAssets.Item[Type].Frame();
		Vector2 origin = frame.Size() / 2f;
		Vector2 itemDrawPosition = Item.position - Main.screenPosition + origin + new Vector2((Item.width / 2) - origin.X, Item.height - frame.Height);
		Vector2 indicatorOffset = new Vector2(-2f, -2f) * scale;
		Texture2D indicatorTexture = TextureAssets.Cursors[CursorOverrideID.FavoriteStar].Value;
		Rectangle indicatorFrame = indicatorTexture.Frame();
		spriteBatch.Draw(indicatorTexture, itemDrawPosition + indicatorOffset + (frame.Size().RotatedBy(rotation) * 0.45f * Item.scale), indicatorFrame, Color.Lerp(alphaColor, Color.Blue, 0.5f), rotation, indicatorFrame.Size() / 2f, 0.5f, SpriteEffects.None, 0f);
	}

	public override ushort GetUniqueDyeIndex()
	{
		return 0;
	}
}