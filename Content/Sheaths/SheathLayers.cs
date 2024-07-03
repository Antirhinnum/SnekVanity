using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SnekVanity.Content.Sheaths;

public abstract class SheathLayer : PlayerDrawLayer
{
	protected abstract (int ItemType, int Dye, Vector2 Offset) GetSettings(PlayerDrawSet drawInfo);

	public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
	{
		return drawInfo.drawPlayer.TryGetModPlayer(out SheathPlayer sheathPlayer) && sheathPlayer.ShouldDrawSheath;
	}

	protected override sealed void Draw(ref PlayerDrawSet drawInfo)
	{
		(int sheath, int dye, Vector2 offset) = GetSettings(drawInfo);
		if (sheath != -1)
		{
			Texture2D sheathTexture = SheathPlayer.KnownSwordsToSheathAssets[sheath].Value;
			Rectangle frame = sheathTexture.Frame(verticalFrames: 20, frameY: drawInfo.drawPlayer.bodyFrame.Y / 56);
			Vector2 position = drawInfo.Position + drawInfo.drawPlayer.legPosition + drawInfo.legVect + offset;

			// The Muramasa has a wider texture than the other sheaths,
			//   so we need to adjust its origin. This generalizes that
			//   to all possible texture widths.
			float extraHorizontalOffset = sheathTexture.Width - 50f;

			DrawData data = new(sheathTexture, (position - Main.screenPosition).Floor(), frame, drawInfo.colorArmorLegs, drawInfo.drawPlayer.legRotation, drawInfo.legVect + new Vector2(extraHorizontalOffset, 0f), 1f, drawInfo.playerEffect) { shader = dye };
			drawInfo.DrawDataCache.Add(data);
			if (SheathPlayer.KnownSwordsToSheathGlowAssets.TryGetValue(sheath, out Asset<Texture2D> glowAsset))
			{
				drawInfo.DrawDataCache.Add(data with { texture = glowAsset.Value, color = Color.White });
			}
		}
	}
}

public sealed class BackSheathLayer : SheathLayer
{
	public override Position GetDefaultPosition()
	{
		// Draws in front of the player's chest, but under their shirt or armor.
		// Can't fix unless I move the chest texture to draw after the back arm, which may have other consequences.
		return new AfterParent(PlayerDrawLayers.Skin);
	}

	protected override (int ItemType, int Dye, Vector2 Offset) GetSettings(PlayerDrawSet drawInfo)
	{
		return !drawInfo.drawPlayer.TryGetModPlayer(out SheathPlayer sheathPlayer)
			? (-1, 0, Vector2.Zero)
			: (sheathPlayer.sheathBack?.type ?? -1, sheathPlayer.cSheathBack, new Vector2(drawInfo.drawPlayer.direction == 1 ? -14f : -16f, 0f));
	}
}

public sealed class FrontSheathLayer : SheathLayer
{
	public override Position GetDefaultPosition()
	{
		return new BeforeParent(PlayerDrawLayers.ArmOverItem);
	}

	protected override (int ItemType, int Dye, Vector2 Offset) GetSettings(PlayerDrawSet drawInfo)
	{
		return !drawInfo.drawPlayer.TryGetModPlayer(out SheathPlayer sheathPlayer)
			? (-1, 0, Vector2.Zero)
			: (sheathPlayer.sheathFront?.type ?? -1, sheathPlayer.cSheathFront, new Vector2(drawInfo.drawPlayer.direction == 1 ? -26f : -4f, 0f));
	}
}