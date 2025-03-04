using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SnekVanity.Content.CombinedDye;

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
		if (data.Player == null || data.Texture == null)
		{
			return;
		}

		Texture2D topTexture = data.Texture;
		if (data.FirstShaderIndex > 0)
		{
			var topTarget = DyeRenderTarget.GetAndRequestTargetInstance(new(data.Player, data.Texture, data.FirstShaderIndex, data.SourceRectangle));
			if (topTarget.IsReady)
			{
				topTexture = topTarget.GetTarget();
			}
		}

		Texture2D bottomTexture = data.Texture;
		if (data.SecondShaderIndex > 0)
		{
			var bottomTarget = DyeRenderTarget.GetAndRequestTargetInstance(new(data.Player, data.Texture, data.SecondShaderIndex, data.SourceRectangle));
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

		int horizontalFrames = Math.Max(1, (int)Math.Floor(data.Texture.Width / (float)data.SourceRectangle.Width));
		int verticalFrames = Math.Max(1, (int)Math.Floor(data.Texture.Height / (float)data.SourceRectangle.Height));
		int frameHeight = data.SourceRectangle.Height;

		// Hack because head textures are weird -- some aren't 1120px tall, and they use a source rectangle with height 52.
		if (data.Texture.Width == 40 && data.Texture.Height >= 1118 && data.Texture.Height <= 1122)
		{
			verticalFrames = 20;
			frameHeight = 56;
		}

		Vector2 realFrameSize = topTexture.Frame(horizontalFrames, verticalFrames).Size();
		realFrameSize.Y = frameHeight;

		for (int i = 0; i < horizontalFrames; i++)
		{
			for (int j = 0; j < verticalFrames; j++)
			{
				Vector2 position = new Vector2(i, j) * realFrameSize;
				Rectangle frame = new((int)position.X, (int)position.Y, (int)realFrameSize.X, (int)realFrameSize.Y);
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