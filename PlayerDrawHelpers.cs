using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;

namespace SnekVanity;

/// <summary>
/// Various helper methods for dealing with player drawing.
/// </summary>
public static class PlayerDrawHelpers
{
	/// <summary>
	/// Determines if the texture in <paramref name="data"/> is the player texture specified by <paramref name="playerTextureID"/> associated with <paramref name="player"/>'s <see cref="Player.skinVariant"/>.
	/// <br/> Shorthand for <c><paramref name="data"/>.texture == TextureAssets.Players[<paramref name="player"/>.skinVariant, <paramref name="playerTextureID"/>].Value</c>
	/// </summary>
	/// <param name="data">The <see cref="DrawData"/> to check the texture of.</param>
	/// <param name="player">The <see cref="Player"/> to check the skin variant of.</param>
	/// <param name="playerTextureID">A value from <see cref="PlayerTextureID"/>.</param>
	public static bool UsesPlayerTexture(DrawData data, Player player, int playerTextureID) => data.texture == TextureAssets.Players[player.skinVariant, playerTextureID].Value;
}