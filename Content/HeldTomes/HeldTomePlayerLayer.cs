using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SnekVanity.Content.HeldTomes;

public sealed class HeldTomePlayerLayer : PlayerDrawLayer
{
	public override Position GetDefaultPosition()
	{
		return new Between(PlayerDrawLayers.Skin, PlayerDrawLayers.Leggings);
	}

	public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
	{
		HeldTomesPlayer heldTomesPlayer = drawInfo.drawPlayer.GetModPlayer<HeldTomesPlayer>();
		return heldTomesPlayer.ShouldDrawTome;
	}

	private static Vector2 GetCompositeOffset_BackArm(ref PlayerDrawSet drawInfo) => new Vector2(6f, 2f) * new Vector2((!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally)).ToDirectionInt(), (!drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically)).ToDirectionInt());

	private static readonly int[] OffsetsPlayerBackArm =
	[
		2,
		2,
		2,
		2,
		2,
		0,
		2,
		4,
		4,
		4,
		2,
		2,
		2,
		2,
		0,
		-2,
		-2,
		0,
		2,
		2
	];

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		HeldTomesPlayer heldTomesPlayer = drawInfo.drawPlayer.GetModPlayer<HeldTomesPlayer>();
		Asset<Texture2D> asset = HeldTomesPlayer._knownTomesToAssets[heldTomesPlayer.heldTomeBack.type];
		Vector2 baseDrawPosition = new Vector2((int)(drawInfo.Position.X - Main.screenPosition.X - (drawInfo.drawPlayer.bodyFrame.Width / 2) + (drawInfo.drawPlayer.width / 2)), (int)(drawInfo.Position.Y - Main.screenPosition.Y + drawInfo.drawPlayer.height - drawInfo.drawPlayer.bodyFrame.Height + 4f)) + drawInfo.drawPlayer.bodyPosition + new Vector2(drawInfo.drawPlayer.bodyFrame.Width / 2, drawInfo.drawPlayer.bodyFrame.Height / 2);

		baseDrawPosition.X += OffsetsPlayerBackArm[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt();
		Vector2 movementOffset = Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height];
		movementOffset.Y -= 2f;
		baseDrawPosition += movementOffset * -drawInfo.playerEffect.HasFlag(SpriteEffects.FlipVertically).ToDirectionInt();
		baseDrawPosition.Y += drawInfo.torsoOffset;
		Vector2 drawPosition = baseDrawPosition;
		Vector2 bodyOrigin = drawInfo.bodyVect;
		Vector2 backArmOffset = GetCompositeOffset_BackArm(ref drawInfo);
		drawPosition += backArmOffset;
		bodyOrigin += backArmOffset;
		float rotation = drawInfo.drawPlayer.bodyRotation + drawInfo.compositeBackArmRotation;

		drawInfo.DrawDataCache.Add(new DrawData(asset.Value, drawPosition, null, drawInfo.colorArmorBody, rotation, bodyOrigin, 1f, drawInfo.playerEffect)
		{
			shader = heldTomesPlayer.cTomeBack
		});
	}
}