using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DyePlayerTextures;

public sealed class AllClothesDyeItem : ModItem, IDyeShirt, IDyeUndershirt, IDyePants, IDyeShoes
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 2));
		Item.vanity = true;
		Item.hasVanityEffects = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<ShirtDyeItem>()
			.AddIngredient<UndershirtDyeItem>()
			.AddIngredient<PantsDyeItem>()
			.AddIngredient<ShoesDyeItem>()
			.AddTile(TileID.DyeVat)
			.Register();
	}
}