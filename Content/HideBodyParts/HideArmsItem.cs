using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.HideBodyParts;

public sealed class HideArmsItem : ModItem, IHideArms, IAmAsymmetricSpecial, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC => NPCID.Clothier;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 75));
		Item.vanity = true;
		Item.hasVanityEffects = true;
	}
}