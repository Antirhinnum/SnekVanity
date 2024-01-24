using SnekVanity.Core;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class NeverClosedContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
	}

	EyeFrame IForceEyeState.SetEyeState(Player player, EyeFrame oldFrame)
	{
		return oldFrame switch
		{
			EyeFrame.EyeClosed => EyeFrame.EyeHalfClosed,
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