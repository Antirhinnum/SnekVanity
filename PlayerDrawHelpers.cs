using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
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

	public static Color GetRawHairDyeColor(int hairDye, Player player, Color? lightColor = null)
	{
		Color oldHairColor = player.hairColor;
		player.hairColor = Color.White;
		Color dyeColor = GameShaders.Hair.GetColor(hairDye, player, lightColor ?? Color.White);
		player.hairColor = oldHairColor;
		return dyeColor;
	}

	public static Rectangle GetRealHairFrameFromTexture(Player player, Texture2D maybeHairTexture, Rectangle? originalFrame)
	{
		return HairID.Sets.DrawBackHair[player.hair] && (TextureAssets.PlayerHair[player.hair].Value == maybeHairTexture || TextureAssets.PlayerHairAlt[player.hair].Value == maybeHairTexture)
			? maybeHairTexture.Frame(verticalFrames: 14)
			: originalFrame ?? maybeHairTexture.Frame();
	}
}