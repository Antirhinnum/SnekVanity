using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.CombinedDye;

public sealed class UselessDyeItem : ModItem, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC { get; } = NPCID.WitchDoctor;

	[field: CloneByReference]
	Condition IAmSoldByVanillaNPC.Available { get; } = Condition.Hardmode;

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.RedDye);
		Item.SetShopValues(ItemRarityColor.TrashMinus1, Item.buyPrice(silver: 1));
		Item.dye = 0;
	}
}