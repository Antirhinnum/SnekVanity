using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.CustomDyes;
using System;
using Terraria;
using Terraria.DataStructures;

namespace SnekVanity.Content.CombinedDye;

/// <summary>
/// Draws a texture under the influence of an arbitrary dye.
/// </summary>
public sealed class DyeRenderTarget : ACachedRenderTarget<DyeRenderTarget, DyeRenderTarget.Data>
{
	public readonly record struct Data
	{
		public Player Player { get; init; }
		public Texture2D Texture { get; init; }
		public int ShaderIndex { get; init; }
		public Rectangle SourceRectangle { get; init; }

		public Data(Player player, Texture2D texture, int shaderIndex, Rectangle? sourceRectangle = null)
		{
			Player = player;
			Texture = texture;
			ShaderIndex = shaderIndex;
			SourceRectangle = PlayerDrawHelpers.GetRealHairFrameFromTexture(player, texture, sourceRectangle) with { X = 0, Y = 0 };
		}
	}

	protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
	{
		if (data.Player == null || data.Texture == null)
		{
			return;
		}

		Texture2D texture = data.Texture;
		int shader = data.ShaderIndex;
		Color color = Color.White;

		CustomDyeHooks.ModifyDrawData(ref texture, ref color, ref shader, data.Player, data.SourceRectangle);
		PlayerDrawHelper.UnpackShader(shader, out int localShader, out PlayerDrawHelper.ShaderConfiguration type);
		if (type == PlayerDrawHelper.ShaderConfiguration.HairShader)
		{
			color = PlayerDrawHelpers.GetRawHairDyeColor(localShader, data.Player);
		}

		PrepareARenderTarget_AndListenToEvents(ref _target, device, texture.Width, texture.Height, RenderTargetUsage.PreserveContents);
		device.SetRenderTarget(_target);
		device.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend); // Need to use Immediate so the shader works

		int horizontalFrames = Math.Max(1, (int)Math.Floor(data.Texture.Width / (float)data.SourceRectangle.Width));
		int verticalFrames = Math.Max(1, (int)Math.Floor(data.Texture.Height / (float)data.SourceRectangle.Height));
		Vector2 realFrameSize = texture.Frame(horizontalFrames, verticalFrames).Size();

		for (int i = 0; i < horizontalFrames; i++)
		{
			for (int j = 0; j < verticalFrames; j++)
			{
				Vector2 position = new Vector2(i, j) * realFrameSize;
				Rectangle frame = new((int)position.X, (int)position.Y, (int)realFrameSize.X, (int)realFrameSize.Y);
				DrawData value = new(texture, position, frame, color) { shader = shader };
				PlayerDrawHelper.SetShaderForData(data.Player, data.Player.cHead, ref value);
				value.Draw(spriteBatch);
			}
		}

		spriteBatch.End();
		device.SetRenderTarget(null);
		_target.Tag = texture;
		_wasPrepared = true;
	}
}