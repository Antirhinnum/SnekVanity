using SnekVanity.Common.Systems;
using SnekVanity.Core;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.Players;

public sealed class HiddenBodyPartsPlayer : ModPlayer
{
	public bool hideFrontArm;

	public bool hideBackArm;

	public bool AnyHiddenBodyParts => hideFrontArm || hideBackArm;

	public override void Load()
	{
		On_Player.UpdateVisibleAccessory += On_Player_UpdateVisibleAccessory;
	}

	public override void ResetEffects()
	{
		hideFrontArm = false;
		hideBackArm = false;
	}

	private void On_Player_UpdateVisibleAccessory(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
	{
		orig(self, itemSlot, item, modded);

		// Bit of a workaround to check exactly which side of the player the item is equipped on.
		// onCorrectSide == front arm hidden
		// !onCorrectSide == back arm hidden
		// both == not asymmetric, both arms hidden
		bool onCorrectSide = CrossModSystem.AsymmetricEquips_ItemOnFrontSide(item, self);
		self.direction = -self.direction;
		bool onWrongSide = CrossModSystem.AsymmetricEquips_ItemOnFrontSide(item, self);
		self.direction = -self.direction;

		if (item.ModItem is IHideArms)
		{
			self.GetModPlayer<HiddenBodyPartsPlayer>().hideFrontArm = onCorrectSide || (onCorrectSide && onWrongSide);
			self.GetModPlayer<HiddenBodyPartsPlayer>().hideBackArm = !onCorrectSide || (onCorrectSide && onWrongSide);
		}
	}
}