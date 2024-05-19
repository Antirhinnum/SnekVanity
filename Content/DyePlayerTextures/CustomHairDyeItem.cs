using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DyePlayerTextures;

public sealed class CustomHairDyeItem : ModItem, IAmSoldByVanillaNPC, IDyeHair
{
	int IAmSoldByVanillaNPC.NPC => NPCID.Stylist;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 5));
		Item.vanity = true;
		Item.hasVanityEffects = true;
	}
}