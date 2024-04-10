using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForcedEyeState;

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will force the player's eyes into a certain state when equipped.
/// </summary>
public interface IForceEyeState
{
	/// <summary>
	/// Sets a player's eye state.
	/// </summary>
	/// <param name="player">The player whose eye state is being set.</param>
	/// <param name="oldFrame">The eye state the player was in before this method was called.</param>
	/// <returns>The new eye state of the player.</returns>
	EyeFrame SetEyeState(Player player, EyeFrame oldFrame);
}