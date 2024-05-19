using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DyePlayerTextures;

public sealed class EyeDyeItem : ModItem, IDyeEyes, IAmAsymmetricSpecial, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC => NPCID.DyeTrader;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 50));
		Item.vanity = true;
		Item.hasVanityEffects = true;
	}
}