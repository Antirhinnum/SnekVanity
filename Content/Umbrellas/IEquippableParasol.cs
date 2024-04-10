using Terraria;

namespace SnekVanity.Content.Umbrellas;

public interface IEquippableParasol
{
	/// <summary>
	/// When <see langword="true"/>, <paramref name="player"/> will have the <see cref="Player.slowFall"/> effect.
	/// </summary>
	bool ActivatesSlowFall(Player player);
}