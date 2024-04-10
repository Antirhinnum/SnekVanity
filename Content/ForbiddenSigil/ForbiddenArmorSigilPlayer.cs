using SnekVanity.Common.PlayerEquips;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForbiddenSigil;

/// <summary>
/// Keeps track of the <see cref="ForbiddenArmorSigilItem"/>.
/// </summary>
public sealed class ForbiddenArmorSigilPlayer : ModPlayer, IAddDyeSlots
{
	public bool sigilActive;
	public int cSigil;

	public override void ResetEffects()
	{
		sigilActive = false;
	}

	public void ClearDyeSlots()
	{
		cSigil = 0;
	}

	public void UpdateDyeSlots(Item armorItem, Item dyeItem)
	{
		if (armorItem.ModItem is ForbiddenArmorSigilItem)
		{
			cSigil = dyeItem.dye;
		}
	}
}