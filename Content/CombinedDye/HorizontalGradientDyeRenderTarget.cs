using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SnekVanity.Content.CombinedDye;

public sealed class HorizontalGradientDyeRenderTarget : ACachedRenderTarget<HorizontalGradientDyeRenderTarget, HorizontalGradientDyeRenderTarget.Data>
{
	private const int MAX_SHADERS = 4;

	public readonly record struct Data
	{
		public Player Player { get; init; }
		public Texture2D Texture { get; init; }
		public int[] ShaderIndices { get; init; }
		public int ShaderCount { get; init; }
		public Rectangle SourceRectangle { get; init; }

		public Data(Player player, Texture2D texture, Rectangle? sourceRectangle = null, params int[] shaderIndices)
		{
			Player = player;
			Texture = texture;
			ShaderIndices = new int[MAX_SHADERS];
			ShaderCount = Math.Min(shaderIndices.Length, MAX_SHADERS);
			Array.Copy(shaderIndices, ShaderIndices, ShaderCount);
			SourceRectangle = PlayerDrawHelpers.GetRealHairFrameFromTexture(player, texture, sourceRectangle);
		}

		public readonly bool Equals(Data other) => Player == other.Player && Texture == other.Texture && ShaderCount == other.ShaderCount && ShaderIndices.SequenceEqual(other.ShaderIndices) && SourceRectangle.Size() == other.SourceRectangle.Size();

		public override readonly int GetHashCode() => HashCode.Combine(Player, Texture, ShaderIndices, SourceRectangle.Size());
	}

	private static Asset<Effect> _horizontalImageGradientAsset;
	private readonly Texture2D[] _textures = new Texture2D[MAX_SHADERS];

	public override void Load(Mod mod)
	{
		base.Load(mod);

		_horizontalImageGradientAsset = ModContent.Request<Effect>("SnekVanity/Assets/Effects/HorizontalImageGradientEffect");
	}

	public override void Unload()
	{
		base.Unload();

		_horizontalImageGradientAsset = null;
	}

	protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
	{
		if (data.Player == null || data.Texture == null)
		{
			return;
		}

		for (int i = 0; i < MAX_SHADERS; i++)
		{
			_textures[i] = data.Texture;
			if (i < data.ShaderCount && data.ShaderIndices[i] != 0)
			{
				ACachedRenderTarget<DyeRenderTarget, DyeRenderTarget.Data> target =
					DyeRenderTarget.GetAndRequestTargetInstance(new(data.Player, data.Texture, data.ShaderIndices[i], data.SourceRectangle));
				if (target.IsReady)
				{
					_textures[i] = target.GetTarget();
				}
			}
		}

		PrepareARenderTarget_AndListenToEvents(ref _target, device, data.Texture.Width, data.Texture.Height, RenderTargetUsage.PreserveContents);
		device.SetRenderTarget(_target);
		device.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

		for (int i = 1; i < Math.Max(2, data.ShaderCount); i++)
		{
			device.Textures[i] = _textures[i];
		}
		_horizontalImageGradientAsset.Value.Parameters["resolution"].SetValue(data.Texture.Size());

		int horizontalFrames = Math.Max(1, (int)Math.Floor(data.Texture.Width / (float)data.SourceRectangle.Width));
		int verticalFrames = Math.Max(1, (int)Math.Floor(data.Texture.Height / (float)data.SourceRectangle.Height));

		string passName = data.ShaderCount switch
		{
			3 => "HorizontalImageGradientEffect3",
			4 => "HorizontalImageGradientEffect4",
			_ => "HorizontalImageGradientEffect2"
		};
		EffectPass pass = _horizontalImageGradientAsset.Value.CurrentTechnique.Passes[passName];
		EffectParameter sourceRectParameter = _horizontalImageGradientAsset.Value.Parameters["sourceRectangle"];

		Vector2 realFrameSize = data.Texture.Frame(horizontalFrames, verticalFrames).Size();

		for (int i = 0; i < horizontalFrames; i++)
		{
			for (int j = 0; j < verticalFrames; j++)
			{
				Vector2 position = new Vector2(i, j) * realFrameSize;
				Rectangle frame = new((int)position.X, (int)position.Y, (int)realFrameSize.X, (int)realFrameSize.Y);
				sourceRectParameter.SetValue(new Vector4(frame.X, frame.Y, frame.Width, frame.Height));
				pass.Apply();
				new DrawData(_textures[0], position, frame, Color.White).Draw(spriteBatch);
			}
		}

		spriteBatch.End();
		device.SetRenderTarget(null);
		_wasPrepared = true;
	}
}