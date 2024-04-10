using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.PlayerEquips;

/// <summary>
/// Any <see cref="ModPlayer"/> that implements this interface will be provided hooks to update and reset equip slots at the same time as vanilla.
/// <br/> It's recommended to use <see cref="IAddDyeSlots"/> as well!
/// </summary>
public interface IAddEquipSlots
{
	/// <summary>
	/// Cache the equip slot associated with <paramref name="item"/>. For example:
	/// <code>
	/// if (item.ModItem is MyEquipItemType equip)
	/// {
	///		this.equipSlot = equip.slot;
	/// }
	/// </code>
	/// </summary>
	/// <param name="item">The <see cref="Item"/> being queried for equip slots.</param>
	void UpdateEquipSlot(Item item);

	/// <summary>
	/// Reset your equip slots. For example:
	/// <code>
	/// this.equipSlot = -1;
	/// </code>
	/// </summary>
	void ResetVisibleAccessories();
}