using SnekVanity.Common.CrossMod.AsymmetricEquips;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForcedEyeState;

public sealed class NeverClosedContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(silver: 50));
		Item.vanity = true;
		Item.hasVanityEffects = true;
	}

	PlayerEyeHelper.EyeFrame IForceEyeState.SetEyeState(Player player, PlayerEyeHelper.EyeFrame oldFrame)
	{
		return oldFrame switch
		{
			PlayerEyeHelper.EyeFrame.EyeClosed => PlayerEyeHelper.EyeFrame.EyeHalfClosed,
			_ => oldFrame
		};
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void RightClick(Player player)
	{
		Item.ChangeItemType(ModContent.ItemType<AlwaysOpenContacts>());
	}

	public override bool ConsumeItem(Player player)
	{
		return false;
	}
}