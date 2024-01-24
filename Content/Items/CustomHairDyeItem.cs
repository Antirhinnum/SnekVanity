using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

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