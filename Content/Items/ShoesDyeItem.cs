using Microsoft.Xna.Framework;
using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class ShoesDyeItem : ModItem, IDyeShoes, IAmSoldByVanillaNPC
{
	public override string Texture => $"Terraria/Images/Item_" + ItemID.Paintbrush;

	int IAmSoldByVanillaNPC.NPC => NPCID.DyeTrader;

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
		Item.color = Color.Purple;
	}
}