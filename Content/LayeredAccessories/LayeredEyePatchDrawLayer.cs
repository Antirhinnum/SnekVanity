using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.LayeredAccessories;

/// <summary>
/// Draws the <see cref="LayeredEyePatchItem"/>.
/// </summary>
public sealed class LayeredEyePatchDrawLayer : PlayerDrawLayer
{
	public override bool IsHeadLayer => true;

	public override Position GetDefaultPosition()
	{
		return new AfterParent(PlayerDrawLayers.Head);
	}

	public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
	{
		return drawInfo.drawPlayer.TryGetModPlayer(out LayeredAccessoriesPlayer layeredAccessoriesPlayer) && layeredAccessoriesPlayer.wearingLayeredEyePatch;
	}

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		for (int i = drawInfo.DrawDataCache.Count - 1; i >= 0; i--)
		{
			// Draw directly over the eyelids. If no eyelids are drawn, assume that the face and eyes are hidden (and thus, so should be eye patch).
			if (!PlayerDrawHelpers.UsesPlayerTexture(drawInfo.DrawDataCache[i], drawInfo.drawPlayer, PlayerTextureID.EyeBlink))
			{
				continue;
			}

			LayeredAccessoriesPlayer layeredAccessoriesPlayer = drawInfo.drawPlayer.GetModPlayer<LayeredAccessoriesPlayer>();
			Main.instance.LoadArmorHead(ArmorIDs.Head.EyePatch); // Not preloaded by vanilla unless the normal eye patch is worn

			// Copied from PlayerDrawLayers::DrawPlayer_21_Head()
			DrawData data = new(TextureAssets.ArmorHead[ArmorIDs.Head.EyePatch].Value, drawInfo.helmetOffset + new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - drawInfo.drawPlayer.bodyFrame.Width / 2 + drawInfo.drawPlayer.width / 2), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.headPosition + drawInfo.headVect, drawInfo.drawPlayer.bodyFrame, drawInfo.colorArmorHead, drawInfo.drawPlayer.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect)
			{
				shader = layeredAccessoriesPlayer.cEyePatch
			};
			drawInfo.DrawDataCache.Insert(i + 1, data);
			break;
		}
	}
}