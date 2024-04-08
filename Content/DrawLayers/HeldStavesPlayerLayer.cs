using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.GlobalItems;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Content.DrawLayers;

// Held stave sprites can be as wide as possible to facilitate them being much longer than the Royal Scepter.
public sealed class HeldStavesPlayerLayer : PlayerDrawLayer
{
	public override Position GetDefaultPosition()
	{
		// Draw after skin since that's where front balloons are drawn
		return new AfterParent(PlayerDrawLayers.Skin);
	}

	public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
	{
		return HeldStavesItem.IsBalloonIdAHeldStave(drawInfo.drawPlayer.balloonFront);
	}

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		Texture2D expectedTexture = TextureAssets.AccBalloon[drawInfo.drawPlayer.balloonFront].Value;
		for (int i = drawInfo.DrawDataCache.Count - 1; i >= 0; i--)
		{
			DrawData data = drawInfo.DrawDataCache[i];
			if (data.texture == expectedTexture)
			{
				if (drawInfo.drawPlayer.direction == -1)
				{
					data.position -= new Vector2(expectedTexture.Width - data.sourceRect.Value.Width, 0f);
				}
				data.sourceRect = data.sourceRect.Value with { Width = expectedTexture.Width };
				drawInfo.DrawDataCache[i] = data;
				break;
			}
		}
	}
}