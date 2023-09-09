using Microsoft.Xna.Framework;
using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class AlwaysClosedContacts : ModItem, IForceEyeState, IAmAsymmetricSpecial, IAmSoldByVanillaNPC
{
	public override string Texture => "Terraria/Images/Item_" + ItemID.Lens;

	int IAmSoldByVanillaNPC.NPC => NPCID.Stylist;

	public override void SetDefaults()
	{
		Item.accessory = true;
		Item.vanity = true;
		Item.value = Item.buyPrice(silver: 50);
		Item.color = Color.Black;
	}

	EyeFrame IForceEyeState.SetEyeState(Player player, EyeFrame oldFrame)
	{
		return EyeFrame.EyeClosed;
	}
}