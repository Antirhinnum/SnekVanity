using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace SnekVanity.Common.PlayerEquips;

/// <summary>
/// Implements the functionality of <see cref="IAddEquipSlots"/> and <see cref="IAddDyeSlots"/>.
/// </summary>
public sealed class PlayerEquipsHooksLoader : ILoadable
{
	private static readonly HookList<ModPlayer> UpdateEquipSlotHook = PlayerLoader.AddModHook(HookList<ModPlayer>.Create(p => ((IAddEquipSlots)p).UpdateEquipSlot));
	private static readonly HookList<ModPlayer> ResetVisibleAccessoriesHook = PlayerLoader.AddModHook(HookList<ModPlayer>.Create(p => ((IAddEquipSlots)p).ResetVisibleAccessories));
	private static readonly HookList<ModPlayer> UpdateDyeSlotsHook = PlayerLoader.AddModHook(HookList<ModPlayer>.Create(p => ((IAddDyeSlots)p).UpdateDyeSlots));
	private static readonly HookList<ModPlayer> ClearDyeSlotsHook = PlayerLoader.AddModHook(HookList<ModPlayer>.Create(p => ((IAddDyeSlots)p).ClearDyeSlots));

	void ILoadable.Load(Mod mod)
	{
		On_Player.UpdateVisibleAccessory += On_Player_UpdateVisibleAccessory;
		On_Player.ResetVisibleAccessories += On_Player_ResetVisibleAccessories;
		On_Player.UpdateDyes += On_Player_UpdateDyes;
		On_Player.UpdateItemDye += On_Player_UpdateItemDye;
	}

	void ILoadable.Unload()
	{
		// Intentionally left blank.
	}

	/// <summary>
	/// Updates the player's equip slots with the given item.
	/// </summary>
	private static void On_Player_UpdateVisibleAccessory(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
	{
		orig(self, itemSlot, item, modded);

		foreach (ModPlayer p in UpdateEquipSlotHook.Enumerate(self))
		{
			(p as IAddEquipSlots).UpdateEquipSlot(item);
		}
	}

	/// <summary>
	/// Reset the player's visible accessories alongside vanilla.
	/// </summary>
	private static void On_Player_ResetVisibleAccessories(On_Player.orig_ResetVisibleAccessories orig, Player self)
	{
		orig(self);

		foreach (ModPlayer p in ResetVisibleAccessoriesHook.Enumerate(self))
		{
			(p as IAddEquipSlots).ResetVisibleAccessories();
		}
	}

	/// <summary>
	/// Updates the player's body dyes from the given item.
	/// </summary>
	private static void On_Player_UpdateItemDye(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
	{
		orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);

		if (isSetToHidden && isNotInVanitySlot)
		{
			return;
		}

		foreach (ModPlayer p in UpdateDyeSlotsHook.Enumerate(self))
		{
			(p as IAddDyeSlots).UpdateDyeSlots(armorItem, dyeItem);
		}
	}

	/// <summary>
	/// Reset the player's dyes alongside vanilla.
	/// </summary>
	private static void On_Player_UpdateDyes(On_Player.orig_UpdateDyes orig, Player self)
	{
		// Do this before orig because UpdateDyes does *all* dye updating, including setting the dye slots to their new values.
		// Clearing dyes happens at the start. Doing it after (or in ModPlayer.UpdateDyes) would clear dyes after they'd been set.
		foreach (ModPlayer p in ClearDyeSlotsHook.Enumerate(self))
		{
			(p as IAddDyeSlots).ClearDyeSlots();
		}

		orig(self);
	}
}