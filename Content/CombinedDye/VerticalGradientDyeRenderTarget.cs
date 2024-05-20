using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SnekVanity.Content.CombinedDye;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SnekVanity.Content.GradientDye;

public sealed class VerticalGradientDyeRenderTarget : ACachedRenderTarget<VerticalGradientDyeRenderTarget, VerticalGradientDyeRenderTarget.Data>
{
	public readonly record struct Data
	{
		public Player Player { get; init; }
		public Texture2D Texture { get; init; }
		public int FirstShaderIndex { get; init; }
		public int SecondShaderIndex { get; init; }
		public Rectangle SourceRectangle { get; init; }

		public Data(Player player, Texture2D texture, int firstShaderIndex, int secondShaderIndex, Rectangle? sourceRectangle = null)
		{
			Player = player;
			Texture = texture;
			FirstShaderIndex = firstShaderIndex;
			SecondShaderIndex = secondShaderIndex;
			SourceRectangle = PlayerDrawHelpers.GetRealHairFrameFromTexture(player, texture, sourceRectangle);
		}

		public readonly bool Equals(Data other) => Player == other.Player && Texture == other.Texture && FirstShaderIndex == other.FirstShaderIndex && SecondShaderIndex == other.SecondShaderIndex && SourceRectangle.Size() == other.SourceRectangle.Size();

		public override readonly int GetHashCode() => HashCode.Combine(Player, Texture, FirstShaderIndex, SecondShaderIndex, SourceRectangle.Size());
	}

	private static Asset<Effect> _vericalImageGradientAsset;

	public override void Load(Mod mod)
	{
		base.Load(mod);

		_vericalImageGradientAsset = ModContent.Request<Effect>("SnekVanity/Assets/Effects/VerticalImageGradientEffect");
	}

	public override void Unload()
	{
		base.Unload();

		_vericalImageGradientAsset = null;
	}

	protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
	{
		if (data.Player == null || data.Texture == null || (data.FirstShaderIndex <= 0 && data.SecondShaderIndex <= 0))
		{
			return;
		}

		Texture2D topTexture = data.Texture;
		if (data.FirstShaderIndex > 0)
		{
			var topTarget = DyeRenderTarget.GetAndRequestTargetInstance(new(data.Player, data.Texture, data.FirstShaderIndex));
			if (topTarget.IsReady)
			{
				topTexture = topTarget.GetTarget();
			}
		}

		Texture2D bottomTexture = data.Texture;
		if (data.SecondShaderIndex > 0)
		{
			var bottomTarget = DyeRenderTarget.GetAndRequestTargetInstance(new(data.Player, data.Texture, data.SecondShaderIndex));
			if (bottomTarget.IsReady)
			{
				bottomTexture = bottomTarget.GetTarget();
			}
		}

		PrepareARenderTarget_AndListenToEvents(ref _target, device, data.Texture.Width, data.Texture.Height, RenderTargetUsage.PreserveContents);
		device.SetRenderTarget(_target);
		device.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

		device.Textures[1] = bottomTexture;
		_vericalImageGradientAsset.Value.Parameters["resolution"].SetValue(topTexture.Size());

		int horizontalFrames = (int)Math.Ceiling(data.Texture.Width / (float)data.SourceRectangle.Width);
		int verticalFrames = (int)Math.Ceiling(data.Texture.Height / (float)data.SourceRectangle.Height);

		for (int i = 0; i < horizontalFrames; i++)
		{
			for (int j = 0; j < verticalFrames; j++)
			{
				Vector2 position = new Vector2(i, j) * data.SourceRectangle.Size();
				Rectangle frame = new(i * data.SourceRectangle.Width, j * data.SourceRectangle.Height, data.SourceRectangle.Width, data.SourceRectangle.Height);
				_vericalImageGradientAsset.Value.Parameters["sourceRectangle"].SetValue(new Vector4(frame.X, frame.Y, frame.Width, frame.Height));
				_vericalImageGradientAsset.Value.CurrentTechnique.Passes["VerticalImageGradientEffect"].Apply();
				new DrawData(topTexture, position, frame, Color.White).Draw(spriteBatch);
			}
		}

		spriteBatch.End();
		device.SetRenderTarget(null);
		_wasPrepared = true;
	}
}