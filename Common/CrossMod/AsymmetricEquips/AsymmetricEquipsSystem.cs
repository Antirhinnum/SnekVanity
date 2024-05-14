using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.CrossMod.AsymmetricEquips;

public sealed class AsymmetricEquipsSystem : ModSystem
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

	internal static bool ItemOnDefaultSide(Item item, Player player)
	{
		return _asymmetricEquips == null || (bool)_asymmetricEquips.Call("ItemOnDefaultSide", item, player);
	}

	internal static void AddEquip(EquipType type, int equipId, int newId = -1, int side = RIGHT_SIDE)
	{
		if (_asymmetricEquips != null)
		{
			_asymmetricEquips.Call("AddEquip", type, equipId, newId, side);
		}
	}

	internal static void AddSpecialItem(int itemId, int side = RIGHT_SIDE)
	{
		if (_asymmetricEquips != null)
		{
			_asymmetricEquips.Call("AddSpecialItem", itemId, side);
		}
	}

	internal static void GetSideInfo(Item item, Player player, out bool notAsymmetric, out bool correctSide, out bool wrongSide)
	{
		correctSide = ItemOnDefaultSide(item, player);
		player.direction = -player.direction;
		wrongSide = ItemOnDefaultSide(item, player);
		player.direction = -player.direction;
		notAsymmetric = correctSide && wrongSide;
	}
}