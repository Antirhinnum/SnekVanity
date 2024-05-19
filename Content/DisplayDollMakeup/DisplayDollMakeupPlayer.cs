using Microsoft.Xna.Framework;
using SnekVanity.Common.PlayerEquips;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace SnekVanity.Content.DisplayDollMakeup;

public sealed class DisplayDollMakeupPlayer : ModPlayer, IAddEquipSlots
{
	private DisplayDollMakeupItem _oldMakeupItem;
	public DisplayDollMakeupItem makeupItem;

	public override void Load()
	{
		On_Player.PlayerFrame += SwapPlayerStyleForPlayerFrame;
		On_LegacyPlayerRenderer.DrawPlayerInternal += SwapPlayerStyleForDrawing;
	}

	private static void SwapPlayerStyleForPlayerFrame(On_Player.orig_PlayerFrame orig, Player self)
	{
		SavedPlayerStyleValues? savedStyle = null;
		if (self.TryGetModPlayer(out DisplayDollMakeupPlayer makeupPlayer) && makeupPlayer._oldMakeupItem != null && makeupPlayer._oldMakeupItem.SavedStyleValues.HasValue)
		{
			savedStyle = new(self);
			makeupPlayer._oldMakeupItem.SavedStyleValues.Value.Retrieve(self);
		}

		orig(self);

		savedStyle?.Retrieve(self);
	}

	private static void SwapPlayerStyleForDrawing(On_LegacyPlayerRenderer.orig_DrawPlayerInternal orig, LegacyPlayerRenderer self, Camera camera, Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow, float alpha, float scale, bool headOnly)
	{
		SavedPlayerStyleValues? savedStyle = null;
		if (drawPlayer.TryGetModPlayer(out DisplayDollMakeupPlayer makeupPlayer) && makeupPlayer._oldMakeupItem != null && makeupPlayer._oldMakeupItem.SavedStyleValues.HasValue)
		{
			savedStyle = new(drawPlayer);
			makeupPlayer._oldMakeupItem.SavedStyleValues.Value.Retrieve(drawPlayer);
		}

		orig(self, camera, drawPlayer, position, rotation, rotationOrigin, shadow, alpha, scale, headOnly);

		savedStyle?.Retrieve(drawPlayer);
	}

	public void ResetVisibleAccessories()
	{
		_oldMakeupItem = makeupItem;
		makeupItem = null;
	}

	public void UpdateEquipSlot(Item item)
	{
		if (item.ModItem is DisplayDollMakeupItem makeup)
		{
			makeupItem = makeup;
		}
	}

	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		if (_oldMakeupItem != null)
		{
			// Fix display dolls using wooden skin for ArmorIDs.Head.Sets.UseSkinColor
			drawInfo.colorDisplayDollSkin = drawInfo.colorHead;
		}
	}
}