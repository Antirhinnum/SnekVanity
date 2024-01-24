using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class AllSkinDyeItem : ModItem, IDyeHeadSkin, IDyeTorsoSkin, IDyeArmSkin, IDyeHandSkin, IDyeLegSkin, IDyeEyeBlink, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC => NPCID.DyeTrader;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
	}
}