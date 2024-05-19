using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SnekVanity.Content.CombinedDye;

/// <summary>
/// An abstraction over <see cref="ARenderTargetContentByRequest"/> that allows caching targets based on their input parameters.
/// </summary>
/// <typeparam name="T">The inheriting class. Used for return values.</typeparam>
/// <typeparam name="D">A class or struct to use as data input.</typeparam>
public abstract class ACachedRenderTarget<T, D> : ARenderTargetContentByRequest, ILoadable, IDisposable, INeedRenderTargetContent
	where T : ACachedRenderTarget<T, D>, new()
{
	internal static List<ACachedRenderTarget<T, D>> _targets;
	private const int MAX_WAIT_FRAMES = 20;
	private byte _framesSinceLastRequest;
	protected D data;

	public virtual void Dispose()
	{
		if (_target != null && !_target!.IsDisposed)
		{
			_target!.Dispose();
			_target = null;
		}

		_targets.Remove(this);
		Main.ContentThatNeedsRenderTargets.Remove(this);
		GC.SuppressFinalize(this);
	}

	public virtual void Load(Mod mod)
	{
		_targets = [];
	}

	public virtual void Unload()
	{
		foreach (ACachedRenderTarget<T, D> t in _targets)
		{
			Main.ContentThatNeedsRenderTargets.Remove(t);
		}

		_targets?.Clear();
		_targets = null;
	}

	void INeedRenderTargetContent.PrepareRenderTarget(GraphicsDevice device, SpriteBatch spriteBatch)
	{
		if (++_framesSinceLastRequest > MAX_WAIT_FRAMES)
		{
			Dispose();
			return;
		}

		PrepareRenderTarget(device, spriteBatch);
	}

	private static ACachedRenderTarget<T, D> AddTarget()
	{
		//Main.NewText("Added target for " + typeof(T));
		T newTarget = new();
		_targets.Add(newTarget);
		Main.ContentThatNeedsRenderTargets.Add(newTarget);
		return newTarget;
	}

	public static ACachedRenderTarget<T, D> GetAndRequestTargetInstance(D data)
	{
		// If there's a target with the same data, use that one.
		// If not, use the first available target that hasn't been requested since it was last drawn.
		// If every available target has been requested since last draw, make a new one.
		//Main.NewText($"Targets for {typeof(T).Name}: " + _targets.Count);
		ACachedRenderTarget<T, D> requestedTarget = _targets.FirstOrDefault(t => data.Equals(t.data));
		if (requestedTarget == null)
		{
			requestedTarget = _targets.FirstOrDefault(t => t._framesSinceLastRequest > MAX_WAIT_FRAMES);
			requestedTarget ??= AddTarget();
		}

		requestedTarget.data = data;
		requestedTarget.Request();
		requestedTarget._framesSinceLastRequest = 0;

		return requestedTarget;
	}
}