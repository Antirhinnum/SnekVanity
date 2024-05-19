using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Terraria.ModLoader;

namespace SnekVanity.Common.CustomDyes;

public sealed class CustomDyeHandler : ILoadable
{
	private static int _nextIndex;
	private static bool _canCacheDatas = false;
	internal static List<ACustomDyeItem> cachedDatas;

	public void Load(Mod mod)
	{
		cachedDatas = [];
		_canCacheDatas = true;
	}

	public void Unload()
	{
		_canCacheDatas = false;
		cachedDatas?.Clear();
		cachedDatas = null;
	}

	public static void CacheItem(ACustomDyeItem item)
	{
		if (!_canCacheDatas)
		{
			return;
		}

		if ((item.Item?.IsAir ?? true) || !item.HasAnyEffects)
		{
			UncacheItem(item);
			return;
		}

		cachedDatas ??= [];

		if (item.cachedDataIndex > 0)
		{
			int index = cachedDatas.FindIndex(i => i.cachedDataIndex == item.cachedDataIndex);
			if (index != -1)
			{
				cachedDatas[index] = item;
				item.Item.dye = item.GetItemDyeValue();
				return;
			}
		}
		item.cachedDataIndex = _nextIndex++;
		cachedDatas.Add(item);
		item.Item.dye = item.GetItemDyeValue();
	}

	internal static void UncacheItem(ACustomDyeItem item)
	{
		cachedDatas?.Remove(item);
	}

	public static bool TryGetDyeFromShaderIndex(int shader, [NotNullWhen(true)] out ACustomDyeItem dye)
	{
		if (cachedDatas == null)
		{
			dye = null;
			return false;
		}

		int supposedShaderIndex = shader >> 16;
		int cachedIndex = shader & 0xFFFF;

		dye = cachedDatas.FirstOrDefault(i => i.cachedDataIndex == cachedIndex);
		return dye != null && supposedShaderIndex == dye.UniqueShaderIndex;
	}
}