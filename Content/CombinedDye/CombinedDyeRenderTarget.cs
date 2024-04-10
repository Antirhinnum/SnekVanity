using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Content.CombinedDye;

// Draws textures under the influence of the first dye.
public sealed class CombinedDyeRenderTarget : ARenderTargetContentByRequest, ILoadable
{
	// We need one target per texture/shader combo, since we replace the textures being used with rendered versions.
	private static List<CombinedDyeRenderTarget> _targets;

	private Player _player;
	private Texture2D _texture;
	private int _shaderIndex;
	private bool _requestedSinceLastDraw;

	public void Load(Mod mod)
	{
		_targets = new();
		for (int i = 0; i < 4; i++)
		{
			AddTarget();
		}
	}

	public void Unload()
	{
		_targets?.Clear();
		_targets = null;
		Main.ContentThatNeedsRenderTargets.RemoveAll(t => t is CombinedDyeRenderTarget);
	}

	private static CombinedDyeRenderTarget AddTarget()
	{
		CombinedDyeRenderTarget newTarget = new();
		_targets.Add(newTarget);
		Main.ContentThatNeedsRenderTargets.Add(newTarget);
		return newTarget;
	}

	public static CombinedDyeRenderTarget GetAndRequestTargetInstance(Player player, Texture2D texture, int shader)
	{
		// If there's a target with the same data, use that one.
		// If not, use the first available target that hasn't been requested since it was last drawn.
		// If every available target has been requested since last draw, make a new one.
		CombinedDyeRenderTarget requestedTarget = _targets.FirstOrDefault(t => t._texture == texture && t._shaderIndex == shader);
		if (requestedTarget == null)
		{
			requestedTarget = _targets.FirstOrDefault(t => !t._requestedSinceLastDraw);
			requestedTarget ??= AddTarget();
		}

		requestedTarget.UseData(player, texture, shader);
		requestedTarget.Request();
		requestedTarget._requestedSinceLastDraw = true;
		return requestedTarget;
	}

	public void UseData(Player player, Texture2D texture, int shader)
	{
		_player = player;
		_texture = texture;
		_shaderIndex = shader;
	}

	protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
	{
		_requestedSinceLastDraw = false;
		if (_player == null || _texture == null || _shaderIndex <= 0)
		{
			return;
		}

		PrepareARenderTarget_AndListenToEvents(ref _target, device, _texture.Width, _texture.Height, RenderTargetUsage.PreserveContents);
		device.SetRenderTarget(_target);
		device.Clear(Color.Transparent);
		DrawData value = new(_texture, Vector2.Zero, Color.White) { shader = _shaderIndex };
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		PlayerDrawHelper.SetShaderForData(_player, _player.cHead, ref value);
		value.Draw(spriteBatch);
		spriteBatch.End();
		device.SetRenderTarget(null);
		_wasPrepared = true;
	}
}