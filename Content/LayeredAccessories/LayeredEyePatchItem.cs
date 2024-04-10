using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.CrossMod.AsymmetricEquips;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.LayeredAccessories;

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
}