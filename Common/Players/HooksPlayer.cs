using SnekVanity.Common.Hooks;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.Players;

public sealed class HooksPlayer : ModPlayer
{
	public override void Load()
	{
		On_Player.UpdateVisibleAccessory += On_Player_UpdateVisibleAccessory;
		On_Player.ResetVisibleAccessories += On_Player_ResetVisibleAccessories;

		On_Player.UpdateDyes += ClearDyeSlots;
		On_Player.UpdateItemDye += UpdatePlayerDye;
	}

	private void On_Player_UpdateVisibleAccessory(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
	{
		orig(self, itemSlot, item, modded);

		IAddEquipSlots.UpdateEquipSlot(self, item);
	}

	private void On_Player_ResetVisibleAccessories(On_Player.orig_ResetVisibleAccessories orig, Player self)
	{
		orig(self);

		IAddEquipSlots.ResetVisibleAccessories(self);
	}

	/// <summary>
	/// Reset the player's dyes alongside vanilla.
	/// </summary>
	private static void ClearDyeSlots(On_Player.orig_UpdateDyes orig, Player self)
	{
		// Do this before orig because UpdateDyes does *all* dye updating, including setting the dye slots to their new values.
		// Clearing dyes happens at the start. Doing it after (or in ModPlayer.UpdateDyes) would clear dyes after they'd been set.
		IAddDyeSlots.ClearDyeSlots(self);

		orig(self);
	}

	/// <summary>
	/// Updates the player's body dyes from the given item.
	/// </summary>
	private void UpdatePlayerDye(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
	{
		orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);

		if ((isSetToHidden && isNotInVanitySlot) || dyeItem.dye == 0)
		{
			return;
		}

		IAddDyeSlots.UpdateDyeSlots(self, armorItem, dyeItem);
	}
}