using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.MiscAccessories;

[AutoloadEquip(EquipType.Waist)]
public sealed class SkirtItem : ModItem, IAmSoldByVanillaNPC
{
	public int NPC { get; } = NPCID.Clothier;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 50));
		Item.vanity = true;
	}
}