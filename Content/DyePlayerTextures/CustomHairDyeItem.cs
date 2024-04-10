using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DyePlayerTextures;

public sealed class CustomHairDyeItem : ModItem, IAmSoldByVanillaNPC, IDyeHair
{
	int IAmSoldByVanillaNPC.NPC => NPCID.Stylist;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.value = Item.buyPrice(gold: 5);
	}
}