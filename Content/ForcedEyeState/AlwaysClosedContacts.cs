using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForcedEyeState;

public sealed class AlwaysClosedContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC => NPCID.Stylist;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 50));
		Item.vanity = true;
		Item.hasVanityEffects = true;
	}

	PlayerEyeHelper.EyeFrame IForceEyeState.SetEyeState(Player player, PlayerEyeHelper.EyeFrame oldFrame)
	{
		return PlayerEyeHelper.EyeFrame.EyeClosed;
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void RightClick(Player player)
	{
		Item.ChangeItemType(ModContent.ItemType<NeverOpenContacts>());
	}

	public override bool ConsumeItem(Player player)
	{
		return false;
	}
}