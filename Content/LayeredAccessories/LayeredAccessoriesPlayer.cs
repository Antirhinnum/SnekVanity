using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.PlayerEquips;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Content.LayeredAccessories;

/// <summary>
/// Keeps track of all layered equips.
/// </summary>
public sealed class LayeredAccessoriesPlayer : ModPlayer, IAddEquipSlots, IAddDyeSlots
{
	public bool wearingLayeredEyePatch;
	public int cEyePatch;

	public void ResetVisibleAccessories()
	{
		wearingLayeredEyePatch = false;
	}

	public void ClearDyeSlots()
	{
		cEyePatch = 0;
	}

	public void UpdateEquipSlot(Item item)
	{
		if (!AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player))
		{
			return;
		}

		if (item.ModItem is LayeredEyePatchItem)
		{
			wearingLayeredEyePatch = true;
		}
	}

	public void UpdateDyeSlots(Item armorItem, Item dyeItem)
	{
		if (dyeItem.dye == 0 || !AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(armorItem, Player))
		{
			return;
		}

		if (armorItem.ModItem is LayeredEyePatchItem)
		{
			cEyePatch = dyeItem.dye;
		}
	}
}