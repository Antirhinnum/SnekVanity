using SnekVanity.Common.CrossMod.AsymmetricEquips;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForcedEyeState;

public sealed class NeverOpenContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial
{
	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
	}

	PlayerEyeHelper.EyeFrame IForceEyeState.SetEyeState(Player player, PlayerEyeHelper.EyeFrame oldFrame)
	{
		return oldFrame switch
		{
			PlayerEyeHelper.EyeFrame.EyeOpen => PlayerEyeHelper.EyeFrame.EyeHalfClosed,
			_ => oldFrame
		};
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void RightClick(Player player)
	{
		Item.ChangeItemType(ModContent.ItemType<AlwaysClosedContacts>());
	}

	public override bool ConsumeItem(Player player)
	{
		return false;
	}
}