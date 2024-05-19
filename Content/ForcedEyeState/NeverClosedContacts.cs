using SnekVanity.Common.CrossMod.AsymmetricEquips;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForcedEyeState;

public sealed class NeverClosedContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
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