using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.CustomDyes;

public sealed class CustomDyeHandler : ModSystem
{
	private static bool _canCacheDatas = false;
	private static readonly List<ACustomDyeItem> _cachedDatas = [];
	private static readonly List<ACustomDyeItem> _dyesToCache = [];
	private static readonly List<ACustomDyeItem> _dyesToUncache = [];

	public override void Load()
	{
		On_Player.UpdateItemDye += CacheCustomDyes;
		Main.OnTickForThirdPartySoftwareOnly += UpdateCachedDyes;
		_canCacheDatas = true;
	}

	private static void UpdateCachedDyes()
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

	private static void CacheCustomDyes(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
	{
		if (dyeItem.ModItem is ACustomDyeItem customDyeItem)
		{
			CacheItem(customDyeItem);
		}

		orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);
	}

	public override void Unload()
	{
		_canCacheDatas = false;
		_cachedDatas.Clear();
		_dyesToCache.Clear();
		_dyesToUncache.Clear();
		Main.OnTickForThirdPartySoftwareOnly -= UpdateCachedDyes;
	}

	public static void CacheItem(ACustomDyeItem dyeItem)
	{
		if (!_canCacheDatas)
		{
			return;
		}

		if (dyeItem.UniqueShaderIndex == -1)
		{
			return;
		}

		if (dyeItem.Item is null || dyeItem.Item.IsAir || !dyeItem.HasAnyEffects)
		{
			// For some reason, uncaching causes dyes to stop working when shown in text snippets.
			//UncacheItem(dyeItem);
			return;
		}

		ushort uniqueIndex = dyeItem.GetUniqueDyeIndex();
		lock (_cachedDatas)
		{
			int index = _cachedDatas.FindIndex(i => i.UniqueShaderIndex == dyeItem.UniqueShaderIndex && i.GetUniqueDyeIndex() == uniqueIndex);
			if (index != -1)
			{
				_cachedDatas[index] = dyeItem;
				dyeItem.Item.dye = dyeItem.GetItemDyeValue();
				return;
			}

			index = _dyesToCache.FindIndex(i => i.UniqueShaderIndex == dyeItem.UniqueShaderIndex && i.GetUniqueDyeIndex() == uniqueIndex);
			if (index != -1)
			{
				_dyesToCache[index] = dyeItem;
				dyeItem.Item.dye = dyeItem.GetItemDyeValue();
				return;
			}
		}

		_dyesToCache.Add(dyeItem);
		dyeItem.Item.dye = dyeItem.GetItemDyeValue();
	}

	internal static void UncacheItem(ACustomDyeItem dyeItem)
	{
		_dyesToUncache.Add(dyeItem);
	}

	public static bool TryGetDyeFromShaderIndex(int shader, [NotNullWhen(true)] out ACustomDyeItem dyeItem)
	{
		lock (_cachedDatas)
		{
			if (_cachedDatas == null)
			{
				dyeItem = null;
				return false;
			}

			int supposedShaderIndex = shader >> 16;
			int cachedIndex = shader & 0xFFFF;

			dyeItem = _cachedDatas.FirstOrDefault(i => i.UniqueShaderIndex == supposedShaderIndex && i.GetUniqueDyeIndex() == cachedIndex);
			return dyeItem != null;
		}
	}
}