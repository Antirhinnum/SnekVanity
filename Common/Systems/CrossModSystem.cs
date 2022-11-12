using SnekVanity.Core;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.Systems;

public sealed class CrossModSystem : ModSystem
{
	private static Mod _asymmetricEquips;

	public override void Unload()
	{
		_asymmetricEquips = null;
	}

	public override void PostSetupContent()
	{
		if (ModLoader.TryGetMod("AsymmetricEquips", out _asymmetricEquips))
		{
			foreach (ModItem item in Mod.GetContent<ModItem>())
			{
				if (item is IAmAsymmetricGlove)
				{
					_asymmetricEquips.Call("AddGlove", item.Type);
				}

				if (item is IAmAsymmetricSpecial)
				{
					_asymmetricEquips.Call("AddSpecialItem", item.Type);
				}
			}
		}
	}

	internal static bool AsymmetricEquips_ItemOnFrontSide(Item item, Player player)
	{
		return _asymmetricEquips == null || (bool)_asymmetricEquips.Call("ItemOnFrontSide", item, player);
	}
}