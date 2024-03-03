using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class AllClothesDyeItem : ModItem, IDyeShirt, IDyeUndershirt, IDyePants, IDyeShoes
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.value = Item.buyPrice(gold: 2);
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