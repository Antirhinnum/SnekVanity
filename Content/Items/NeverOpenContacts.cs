﻿using Microsoft.Xna.Framework;
using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class NeverOpenContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial, IAmSoldByVanillaNPC
{
	public override string Texture => "Terraria/Images/Item_" + ItemID.Lens;

	int IAmSoldByVanillaNPC.NPC => NPCID.Stylist;

	public override void SetStaticDefaults()
	{
		SacrificeTotal = 1;
	}

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.canBePlacedInVanityRegardlessOfConditions = true;
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
		Item.color = Color.Red;
	}

	EyeFrame IForceEyeState.SetEyeState(Player player, EyeFrame oldFrame)
	{
		return oldFrame switch
		{
			EyeFrame.EyeOpen => EyeFrame.EyeHalfClosed,
			_ => oldFrame
		};
	}
}