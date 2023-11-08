using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace SnekVanity.Common.Hooks;

public interface IAddEquipSlots : IAddDyeSlots
{
	public static readonly HookList<ModPlayer> UpdateEquipSlotHook = PlayerLoader.AddModHook(new HookList<ModPlayer>(typeof(IAddEquipSlots).GetMethod(nameof(UpdateEquipSlot), new Type[] { typeof(Item) })));
	public static readonly HookList<ModPlayer> ResetVisibleAccessoriesHook = PlayerLoader.AddModHook(new HookList<ModPlayer>(typeof(IAddEquipSlots).GetMethod(nameof(ResetVisibleAccessories), Type.EmptyTypes)));

	void UpdateEquipSlot(Item item);

	void ResetVisibleAccessories();

	public static void UpdateEquipSlot(Player player, Item item)
	{
		foreach (ModPlayer p in UpdateEquipSlotHook.Enumerate(player))
		{
			(p as IAddEquipSlots).UpdateEquipSlot(item);
		}
	}

	public static void ResetVisibleAccessories(Player player)
	{
		foreach (ModPlayer p in ResetVisibleAccessoriesHook.Enumerate(player))
		{
			(p as IAddEquipSlots).ResetVisibleAccessories();
		}
	}
}