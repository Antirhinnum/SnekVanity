using SnekVanity.Common.Hooks;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.Players;

public sealed class DyeHooksPlayer : ModPlayer
{
	public override void Load()
	{
		On_Player.UpdateDyes += ClearDyeSlots;
		On_Player.UpdateItemDye += UpdatePlayerDye;
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

		if (isSetToHidden && isNotInVanitySlot)
		{
			return;
		}

		IAddDyeSlots.UpdateDyeSlots(self, armorItem, dyeItem);
	}
}