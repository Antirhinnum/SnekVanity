using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.PlayerEquips;

/// <summary>
/// Any <see cref="ModPlayer"/> that implements this interface will be provided hooks to update and reset dye slots at the same time as vanilla.
/// </summary>
public interface IAddDyeSlots
{
	/// <summary>
	/// Cache the dye slot associated with <paramref name="armorItem"/>. For example:
	/// <code>
	/// if (item.ModItem is MyDyeSlotItem)
	/// {
	///		this.dyeSlot = dyeItem.dye;
	/// }
	/// </code>
	/// </summary>
	/// <param name="armorItem">The <see cref="Item"/> being dyed.</param>
	/// <param name="dyeItem">The dye <see cref="Item"/>.</param>
	void UpdateDyeSlots(Item armorItem, Item dyeItem);

	/// <summary>
	/// Reset your dye slots. For example:
	/// <code>
	/// this.dyeSlot = 0;
	/// </code>
	/// </summary>
	void ClearDyeSlots();
}