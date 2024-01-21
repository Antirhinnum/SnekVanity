using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DrawLayers;

public abstract class ShoulderBirdLayer : PlayerDrawLayer
{
	protected abstract (int Bird, int Dye, Vector2 Offset) GetSettings(PlayerDrawSet drawInfo);

	protected override sealed void Draw(ref PlayerDrawSet drawInfo)
	{
		if (drawInfo.shadow > 0f)
		{
			return;
		}

		SpriteEffects birdEffect = drawInfo.playerEffect ^ SpriteEffects.FlipHorizontally;
		(int bird, int dye, Vector2 offset) = GetSettings(drawInfo);

		if (bird != -1)
		{
			if (bird < NPCID.Count)
			{
				Main.instance.LoadNPC(bird); // Load the bird texture if it isn't loaded yet. Fixes an issue where birds wouldn't show up when you first entered a world.
			}

			Texture2D birdTexture = TextureAssets.Npc[bird].Value;
			Rectangle frame = birdTexture.Frame(verticalFrames: Main.npcFrameCount[bird]);
			Vector2 position = drawInfo.Position + drawInfo.drawPlayer.bodyPosition + drawInfo.bodyVect + offset;
			position += Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];

			drawInfo.DrawDataCache.Add(new(birdTexture, (position - Main.screenPosition).Floor(), frame, drawInfo.colorArmorBody, 0f, frame.Bottom(), 1f, birdEffect) { shader = dye });
		}
	}
}

public sealed class BackShoulderBirdLayer : ShoulderBirdLayer
{
	public override Position GetDefaultPosition()
	{
		// If I use BeforeParent here, then the layer is counted as a head layer.
		return new Between(PlayerDrawLayers.FirstVanillaLayer, PlayerDrawLayers.HairBack);
	}

	protected override (int Bird, int Dye, Vector2 Offset) GetSettings(PlayerDrawSet drawInfo)
	{
		return !drawInfo.drawPlayer.TryGetModPlayer(out ShoulderBirdPlayer birdPlayer)
			? (-1, -1, Vector2.Zero)
			: (birdPlayer.birdNpcId, birdPlayer.cBird, new Vector2(drawInfo.drawPlayer.direction == 1 ? -7f : -15f, 0f));
	}
}

public sealed class FrontShoulderBirdLayer : ShoulderBirdLayer
{
	public override Position GetDefaultPosition()
	{
		// If I use AfterParent here, then the layer is counted as a head layer.
		return new Between(PlayerDrawLayers.FaceAcc, PlayerDrawLayers.LastVanillaLayer);
	}

	protected override (int Bird, int Dye, Vector2 Offset) GetSettings(PlayerDrawSet drawInfo)
	{
		return !drawInfo.drawPlayer.TryGetModPlayer(out ShoulderBirdPlayer birdPlayer)
			? (-1, -1, Vector2.Zero)
			: (birdPlayer.birdFrontNpcId, birdPlayer.cBirdFront, new Vector2(drawInfo.drawPlayer.direction == 1 ? -19f : -1f, 0f));
	}
}