﻿using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class AlwaysClosedContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial, IAmSoldByVanillaNPC
{
	int IAmSoldByVanillaNPC.NPC => NPCID.Stylist;

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
	}

	EyeFrame IForceEyeState.SetEyeState(Player player, EyeFrame oldFrame)
	{
		return EyeFrame.EyeClosed;
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