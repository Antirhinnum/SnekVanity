using SnekVanity.Common.ShopSelling;
using SnekVanity.Content.DisplayDollMakeup;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DyePlayerTextures;

public sealed class AllSkinDyeItem : ModItem, IDyeHeadSkin, IDyeTorsoSkin, IDyeArmSkin, IDyeHandSkin, IDyeLegSkin, IDyeEyeBlink, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC => NPCID.DyeTrader;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<DisplayDollMakeupItem>();
	}

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 50));
		Item.vanity = true;
		Item.hasVanityEffects = true;
	}
}