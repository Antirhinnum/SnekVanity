using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Terraria.ModLoader;

namespace SnekVanity.Common.CustomDyes;

public sealed class CustomDyeHandler : ModSystem
{
	private static int _nextIndex;
	private static bool _canCacheDatas = false;
	private static readonly List<ACustomDyeItem> _cachedDatas = [];
	private static readonly List<ACustomDyeItem> _dyesToCache = [];
	private static readonly List<ACustomDyeItem> _dyesToUncache = [];

	public override void Load()
	{
		_canCacheDatas = true;
	}

	public override void Unload()
	{
		_canCacheDatas = false;
		_cachedDatas.Clear();
		_dyesToCache.Clear();
		_dyesToUncache.Clear();
	}

	public override void PreUpdatePlayers()
	{
		if (!_canCacheDatas)
		{
			return;
		}

		lock (_cachedDatas)
		{
			if (_dyesToCache?.Count > 0)
			{
				_cachedDatas.AddRange(_dyesToCache);
				_dyesToCache.Clear();
			}

			if (_dyesToUncache?.Count > 0)
			{
				foreach (ACustomDyeItem dyeItem in _dyesToUncache)
				{
					_cachedDatas.Remove(dyeItem);
				}
				_dyesToUncache.Clear();
			}
		}
	}

	public static void CacheItem(ACustomDyeItem dyeItem)
	{
		if (!_canCacheDatas)
		{
			return;
		}

		if ((dyeItem.Item?.IsAir ?? true) || !dyeItem.HasAnyEffects)
		{
			UncacheItem(dyeItem);
			return;
		}

		if (dyeItem.cachedDataIndex > 0)
		{
			int index = _cachedDatas.FindIndex(i => i.cachedDataIndex == dyeItem.cachedDataIndex);
			if (index != -1)
			{
				_cachedDatas[index] = dyeItem;
				dyeItem.Item.dye = dyeItem.GetItemDyeValue();
				return;
			}
		}

		dyeItem.cachedDataIndex = _nextIndex++;
		_dyesToCache.Add(dyeItem);
		dyeItem.Item.dye = dyeItem.GetItemDyeValue();
	}

	internal static void UncacheItem(ACustomDyeItem dyeItem)
	{
		_dyesToUncache.Add(dyeItem);
	}

	public static bool TryGetDyeFromShaderIndex(int shader, [NotNullWhen(true)] out ACustomDyeItem dyeItem)
	{
		if (_cachedDatas == null)
		{
			dyeItem = null;
			return false;
		}

		int supposedShaderIndex = shader >> 16;
		int cachedIndex = shader & 0xFFFF;

		dyeItem = _cachedDatas.FirstOrDefault(i => i.cachedDataIndex == cachedIndex);
		return dyeItem != null && supposedShaderIndex == dyeItem.UniqueShaderIndex;
	}
}