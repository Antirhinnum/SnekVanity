using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.HeldStaves;

[AutoloadEquip(EquipType.Balloon)]
public sealed class WalkingStickItem : ModItem
{
	public override void SetStaticDefaults()
	{
		ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[Item.balloonSlot] = true;
		ArmorIDs.Balloon.Sets.UsesTorsoFraming[Item.balloonSlot] = true;
		HeldStavesItem.AddStaveItem(Item);
	}

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		player.GetModPlayer<WalkingStickPlayer>().hasWalkingStick = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddRecipeGroup(RecipeGroupID.Wood, 9)
			.AddTile(TileID.WorkBenches)
			.Register()
			.SortBeforeFirstRecipesOf(ModContent.ItemType<BowstaffItem>());
	}
}