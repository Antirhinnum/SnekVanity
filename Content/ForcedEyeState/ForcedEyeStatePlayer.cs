using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.PlayerEquips;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForcedEyeState;

public sealed class ForcedEyeStatePlayer : ModPlayer, IAddEquipSlots
{
	private static MethodInfo _PlayerEyeHelper_set_EyeFrameToShow;
	public IForceEyeState eyeState;

	public override void Load()
	{
		_PlayerEyeHelper_set_EyeFrameToShow = typeof(PlayerEyeHelper).GetProperty(nameof(PlayerEyeHelper.EyeFrameToShow)).GetSetMethod(nonPublic: true);
		On_PlayerEyeHelper.UpdateEyeFrameToShow += ForceEyeState;
	}

	public override void Unload()
	{
		_PlayerEyeHelper_set_EyeFrameToShow = null;
	}

	public void UpdateEquipSlot(Item item)
	{
		if (!AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player))
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
		// Intentionally left blank, eyeState is reset below in ForceEyeState()
	}

	/// <summary>
	/// Does exactly what it says on the tin.
	/// </summary>
	private static void ForceEyeState(On_PlayerEyeHelper.orig_UpdateEyeFrameToShow orig, ref PlayerEyeHelper self, Player player)
	{
		orig(ref self, player);

		if (_PlayerEyeHelper_set_EyeFrameToShow != null && player.TryGetModPlayer(out ForcedEyeStatePlayer fPlayer) && fPlayer.eyeState != null)
		{
			// The pains of reflection on immutable structs.
			int newFrame = (int)fPlayer.eyeState!.SetEyeState(player, (EyeFrame)self.EyeFrameToShow);
			object temp = self;
			_PlayerEyeHelper_set_EyeFrameToShow.Invoke(temp, new object[] { newFrame });
			self = (PlayerEyeHelper)temp;

			// Reset eyeState here since it's never read again this frame.
			fPlayer.eyeState = null;
		}
	}
}