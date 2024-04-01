using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace SnekVanity.Common.Hooks;

public interface IAddDyeSlots
{
	public static readonly HookList<ModPlayer> UpdateDyeSlotsHook = PlayerLoader.AddModHook(HookList<ModPlayer>.Create(p => ((IAddDyeSlots)p).UpdateDyeSlots));
	public static readonly HookList<ModPlayer> ClearDyeSlotsHook = PlayerLoader.AddModHook(HookList<ModPlayer>.Create(p => ((IAddDyeSlots)p).ClearDyeSlots));

	void UpdateDyeSlots(Item armorItem, Item dyeItem);

	void ClearDyeSlots();

	public static void UpdateDyeSlots(Player player, Item armorItem, Item dyeItem)
	{
		foreach (ModPlayer p in ClearDyeSlotsHook.Enumerate(player))
		{
			(p as IAddDyeSlots).UpdateDyeSlots(armorItem, dyeItem);
		}
	}

	public static void ClearDyeSlots(Player player)
	{
		foreach (ModPlayer p in ClearDyeSlotsHook.Enumerate(player))
		{
			(p as IAddDyeSlots).ClearDyeSlots();
		}
	}
}