using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.PlayerEquips;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Content.HideBodyParts;

public sealed class HiddenBodyPartsPlayer : ModPlayer, IAddEquipSlots
{
	public bool hideFrontArm;

	public bool hideBackArm;

	public bool AnyHiddenBodyParts => hideFrontArm || hideBackArm;

	public void ResetVisibleAccessories()
	{
		hideFrontArm = false;
		hideBackArm = false;
	}

	public void UpdateEquipSlot(Item item)
	{
		if (item.ModItem is IHideArms)
		{
			AsymmetricEquipsSystem.GetSideInfo(item, Player, out bool notAsymmetric, out bool correctSide, out _);
			hideFrontArm = correctSide || notAsymmetric;
			hideBackArm = !correctSide || notAsymmetric;
		}
	}
}