using Microsoft.Xna.Framework;
using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class CustomHairDyeItem : ModItem, IAmSoldByVanillaNPC, IDyeHair
{
	public override string Texture => $"Terraria/Images/Item_" + ItemID.HairDyeRemover;

	int IAmSoldByVanillaNPC.NPC => NPCID.Stylist;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.vanity = true;
		Item.value = Item.buyPrice(gold: 5);
		Item.color = Color.CornflowerBlue;
	}
}