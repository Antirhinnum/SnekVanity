using Terraria.ModLoader;

namespace SnekVanity.Common.CrossMod.AsymmetricEquips;

/// <summary>
/// A <see cref="ModItem"/> that implements this interface will use glove asymmetry when the mod "Asymmetric Equips" is enabled.
/// </summary>
public interface IAmAsymmetricGlove
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will use manual asymmetry when the mod "Asymmetric Equips" is enabled.
/// </summary>
public interface IAmAsymmetricSpecial
{
	int AsymmetricDefaultSide => AsymmetricEquipsSystem.RIGHT_SIDE;
}