using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForbiddenSigil;

public sealed class ForbiddenArmorSigilItem : ModItem
{
	public override string Texture => $"Terraria/Images/Extra_{ExtrasID.ForbiddenSign}";

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.hasVanityEffects = true;
		Item.value = Item.sellPrice(gold: 2);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (!hideVisual)
		{
			player.GetModPlayer<ForbiddenArmorSigilPlayer>().sigilActive = true;
		}
	}

	public override void UpdateVanity(Player player)
	{
		player.GetModPlayer<ForbiddenArmorSigilPlayer>().sigilActive = true;
	}

	public override void AddRecipes()
	{
		Recipe firstRecipe = CreateRecipe()
			.AddIngredient(ItemID.AncientBattleArmorMaterial)
			.AddIngredient(ItemID.AdamantiteBar, 4)
			.AddTile(TileID.MythrilAnvil)
			.SortAfter(Main.recipe.First(r => r.HasIngredient(ItemID.TitaniumBar) && r.HasResult(ItemID.AncientBattleArmorPants))) // Try to ensure this is after vanilla's recipe and not some alternative mod recipe
			.Register();

		CreateRecipe()
			.AddIngredient(ItemID.AncientBattleArmorMaterial)
			.AddIngredient(ItemID.TitaniumBar, 4)
			.AddTile(TileID.MythrilAnvil)
			.SortAfter(firstRecipe)
			.DisableDecraft()
			.Register();
	}
}