using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace SnekVanity.Common.Hooks;

public interface IAddDyeSlots
{
	public static readonly HookList<ModPlayer> UpdateDyeSlotsHook = PlayerLoader.AddModHook(new HookList<ModPlayer>(typeof(IAddDyeSlots).GetMethod(nameof(UpdateDyeSlots), new Type[] { typeof(Item), typeof(Item) })));
	public static readonly HookList<ModPlayer> ClearDyeSlotsHook = PlayerLoader.AddModHook(new HookList<ModPlayer>(typeof(IAddDyeSlots).GetMethod(nameof(ClearDyeSlots), Type.EmptyTypes)));

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