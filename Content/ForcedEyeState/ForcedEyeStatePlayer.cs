using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.PlayerEquips;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForcedEyeState;

public sealed class ForcedEyeStatePlayer : ModPlayer, IAddEquipSlots
{
	public IForceEyeState eyeState;

	public void UpdateEquipSlot(Item item)
	{
		if (!AsymmetricEquipsSystem.ItemOnDefaultSide(item, Player))
		{
			return;
		}

		if (item.ModItem is IForceEyeState forceItem)
		{
			eyeState = forceItem;
		}
	}

	public void ResetVisibleAccessories()
	{
		eyeState = null;
	}

	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		if (eyeState != null)
		{
			Player.eyeHelper.CurrentEyeFrame = eyeState.SetEyeState(Player, Player.eyeHelper.CurrentEyeFrame);
		}
		else if (Player.isDisplayDollOrInanimate)
		{
			// Make sure display dolls don't get stuck with closed eyes.
			Player.eyeHelper.CurrentEyeFrame = PlayerEyeHelper.EyeFrame.EyeOpen;
		}
	}
}