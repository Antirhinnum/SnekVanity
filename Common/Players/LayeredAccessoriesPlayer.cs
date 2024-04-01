using SnekVanity.Common.Hooks;
using SnekVanity.Common.Systems;
using SnekVanity.Content.Items;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.Players;

public sealed class LayeredAccessoriesPlayer : ModPlayer, IAddEquipSlots
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
		if (!CrossModSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player))
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
		if (dyeItem.dye == 0 || !CrossModSystem.AsymmetricEquips_ItemOnDefaultSide(armorItem, Player))
		{
			return;
		}

		if (armorItem.ModItem is LayeredEyePatchItem)
		{
			cEyePatch = dyeItem.dye;
		}
	}
}