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
		// Bit of a workaround to check exactly which side of the player the item is equipped on.
		// onCorrectSide == front arm hidden
		// !onCorrectSide == back arm hidden
		// both == not asymmetric, both arms hidden
		bool onCorrectSide = AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player);
		Player.direction = -Player.direction;
		bool onWrongSide = AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player);
		Player.direction = -Player.direction;

		if (item.ModItem is IHideArms)
		{
			hideFrontArm = onCorrectSide || onCorrectSide && onWrongSide;
			hideBackArm = !onCorrectSide || onCorrectSide && onWrongSide;
		}
	}
}