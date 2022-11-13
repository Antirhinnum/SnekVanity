using SnekVanity.Common.Systems;
using SnekVanity.Core;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Common.Players;

public sealed class ForcedEyeStatePlayer : ModPlayer
{
	private static PropertyInfo _PlayerEyeHelper_EyeFrameToShow;
	public IForceEyeState eyeState;

	public override void Load()
	{
		_PlayerEyeHelper_EyeFrameToShow = typeof(PlayerEyeHelper).GetProperty(nameof(PlayerEyeHelper.EyeFrameToShow));
		On.Terraria.Player.UpdateVisibleAccessory += UpdateAccessoryVisuals;
		On.Terraria.GameContent.PlayerEyeHelper.UpdateEyeFrameToShow += ForceEyeState;
	}

	public override void Unload()
	{
		_PlayerEyeHelper_EyeFrameToShow = null;
		On.Terraria.Player.UpdateVisibleAccessory -= UpdateAccessoryVisuals;
		On.Terraria.GameContent.PlayerEyeHelper.UpdateEyeFrameToShow -= ForceEyeState;
	}

	/// <summary>
	/// Updates <see cref="eyeState"/> when an accessory is visible.
	/// </summary>
	private static void UpdateAccessoryVisuals(On.Terraria.Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
	{
		orig(self, itemSlot, item, modded);

		if (!CrossModSystem.AsymmetricEquips_ItemOnFrontSide(item, self))
		{
			return;
		}

		if (self.TryGetModPlayer(out ForcedEyeStatePlayer fPlayer) && item.ModItem is IForceEyeState forceItem)
		{
			fPlayer.eyeState = forceItem;
		}
	}

	/// <summary>
	/// Does exactly what it says on the tin.
	/// </summary>
	private static void ForceEyeState(On.Terraria.GameContent.PlayerEyeHelper.orig_UpdateEyeFrameToShow orig, ref PlayerEyeHelper self, Player player)
	{
		orig(ref self, player);

		if (_PlayerEyeHelper_EyeFrameToShow != null && player.TryGetModPlayer(out ForcedEyeStatePlayer fPlayer) && fPlayer.eyeState != null)
		{
			// The pains of reflection on immutable structs.
			int newFrame = (int)fPlayer.eyeState!.SetEyeState(player, (EyeFrame)self.EyeFrameToShow);
			object temp = self;
			_PlayerEyeHelper_EyeFrameToShow.SetValue(temp, newFrame);
			self = (PlayerEyeHelper)temp;

			// Reset eyeState here since it's never read again this frame.
			fPlayer.eyeState = null;
		}
	}
}