using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.CustomDyes;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace SnekVanity.Content.OtherShadersAsDyes;

public sealed class EchoCoatingDyeItem : ACustomDyeItem
{
	public override string Texture { get; } = $"Terraria/Images/Item_{ItemID.EchoCoating}";
	public override bool HasAnyEffects => !Main.ShouldShowInvisibleWalls();

	public override void ModifyDrawData(ref Texture2D texture, ref Color color, ref int shader, Player associatedPlayer, Rectangle? sourceRectangle = null)
	{
		color = Color.Transparent;
	}

	public override void AddRecipes()
	{
		Recipe recipe = CreateRecipe()
			.AddIngredient(ItemID.EchoCoating)
			.AddTile(TileID.DyeVat)
			.Register()
			.SortAfterFirstRecipesOf(ItemID.DeepPinkPaint);

		Recipe.Create(ItemID.EchoCoating)
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
}
