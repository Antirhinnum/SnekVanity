using SnekVanity.Core;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class NeverOpenContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial
{
	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
	}

	EyeFrame IForceEyeState.SetEyeState(Player player, EyeFrame oldFrame)
	{
		return oldFrame switch
		{
			EyeFrame.EyeOpen => EyeFrame.EyeHalfClosed,
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