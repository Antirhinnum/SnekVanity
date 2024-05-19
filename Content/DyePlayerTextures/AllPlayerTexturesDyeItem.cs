using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DyePlayerTextures;

public sealed class AllPlayerTexturesDyeItem : ModItem, IDyeHeadSkin, IDyeTorsoSkin, IDyeArmSkin, IDyeHandSkin, IDyeLegSkin, IDyeEyeBlink, IDyeShirt, IDyeUndershirt, IDyePants, IDyeShoes, IDyeEyes, IDyeEyeWhites, IDyeHair
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 3, silver: 50));
		Item.vanity = true;
		Item.hasVanityEffects = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<AllClothesDyeItem>()
			.AddIngredient<AllSkinDyeItem>()
			.AddIngredient<EyeDyeItem>()
			.AddIngredient<ScleraDyeItem>()
			.AddTile(TileID.DyeVat)
			.SortAfterFirstRecipesOf(ModContent.ItemType<AllClothesDyeItem>())
			.Register();
	}
}