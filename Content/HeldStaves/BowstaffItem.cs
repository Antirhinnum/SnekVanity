using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.HeldStaves;

[AutoloadEquip(EquipType.Balloon)]
public sealed class BowstaffItem : ModItem
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
		Item.vanity = true;
		Item.SetWeaponValues(4, 3f);
		Item.SetShopValues(ItemRarityColor.White0, 0);
		Item.DamageType = DamageClass.Melee;
		Item.shoot = ModContent.ProjectileType<BowstaffProjectile>();
		Item.shootSpeed = 24f;
		Item.useStyle = ItemUseStyleID.Shoot;
		Item.useAnimation = 30;
		Item.useTime = 30;
		Item.UseSound = SoundID.DD2_SkyDragonsFurySwing;
		Item.noMelee = true;
		Item.noUseGraphic = true;
		Item.channel = true;
		Item.autoReuse = true;
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient<WalkingStickItem>()
			.AddIngredient(ItemID.StoneBlock, 5)
			.AddTile(TileID.Anvils)
			.Register()
			.SortAfterFirstRecipesOf(ModContent.ItemType<WalkingStickItem>());
	}
}