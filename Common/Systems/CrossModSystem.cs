using SnekVanity.Core;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.Systems;

public sealed class CrossModSystem : ModSystem
{
	internal const int LEFT_SIDE = 1;
	internal const int RIGHT_SIDE = 2;

	private static Mod _asymmetricEquips;

	public override void Load()
	{
		ModLoader.TryGetMod("AsymmetricEquips", out _asymmetricEquips);
	}

	public override void Unload()
	{
		_asymmetricEquips = null;
	}

	public override void PostSetupContent()
	{
		if (_asymmetricEquips != null)
		{
			foreach (ModItem item in Mod.GetContent<ModItem>())
			{
				if (item is IAmAsymmetricGlove)
				{
					_asymmetricEquips.Call("AddGlove", item.Type);
				}

				if (item is IAmAsymmetricSpecial asymmetricSpecial)
				{
					_asymmetricEquips.Call("AddSpecialItem", item.Type, asymmetricSpecial.AsymmetricDefaultSide);
				}
			}
		}
	}

	internal static bool AsymmetricEquips_ItemOnDefaultSide(Item item, Player player)
	{
		return _asymmetricEquips == null || (bool)_asymmetricEquips.Call("ItemOnDefaultSide", item, player);
	}

	internal static void AsymmetricEquips_AddSpecialItem(int itemId, int side = RIGHT_SIDE)
	{
		if (_asymmetricEquips != null)
		{
			_asymmetricEquips.Call("AddSpecialItem", itemId, side);
		}
	}
}