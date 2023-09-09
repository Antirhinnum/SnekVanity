﻿using Microsoft.Xna.Framework;
using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class AllSkinDyeItem : ModItem, IDyeHeadSkin, IDyeTorsoSkin, IDyeArmSkin, IDyeHandSkin, IDyeLegSkin, IDyeEyeBlink, IAmSoldByVanillaNPC
{
	public override string Texture => $"Terraria/Images/Item_" + ItemID.PaintRoller;

	int IAmSoldByVanillaNPC.NPC => NPCID.DyeTrader;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
		Item.color = Color.Gray;
	}
}