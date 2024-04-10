using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.HideBodyParts;

public sealed class HideArmsItem : ModItem, IHideArms, IAmAsymmetricSpecial, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC => NPCID.Clothier;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 75);
	}
}