using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DyePlayerTextures;

public sealed class ShirtDyeItem : ModItem, IDyeShirt, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC => NPCID.DyeTrader;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
	}
}