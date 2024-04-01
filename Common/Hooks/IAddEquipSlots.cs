using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace SnekVanity.Common.Hooks;

public interface IAddEquipSlots : IAddDyeSlots
{
	public static readonly HookList<ModPlayer> UpdateEquipSlotHook = PlayerLoader.AddModHook(HookList<ModPlayer>.Create(p => ((IAddEquipSlots)p).UpdateEquipSlot));
	public static readonly HookList<ModPlayer> ResetVisibleAccessoriesHook = PlayerLoader.AddModHook(HookList<ModPlayer>.Create(p => ((IAddEquipSlots)p).ResetVisibleAccessories));

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