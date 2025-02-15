using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

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

	// Mostly copied from LegacyPlayerRenderer::DrawPlayerInternal
	public static void GetPlayerDrawData(this Player drawPlayer, Vector2 position, out List<DrawData> drawData, out List<int> dust, out List<int> gores, float rotation = 0f, Vector2 rotationOrigin = default, bool headOnly = false, float alpha = 1f, float shadow = 0f, float scale = 1f)
	{
		drawData = new();
		dust = new();
		gores = new();

		if (drawPlayer.ShouldNotDraw)
		{
			return;
		}

		PlayerDrawSet drawInfo = default;
		if (headOnly)
		{
			drawInfo.HeadOnlySetup(drawPlayer, drawData, dust, gores, position.X, position.Y, alpha, scale);
		}
		else
		{
			drawInfo.BoringSetup(drawPlayer, drawData, dust, gores, position, shadow, rotation, rotationOrigin);
		}

		PlayerLoader.ModifyDrawInfo(ref drawInfo);

		foreach (var layer in PlayerDrawLayerLoader.GetDrawLayers(drawInfo))
		{
			if (!headOnly || layer.IsHeadLayer)
			{
				layer.DrawWithTransformationAndChildren(ref drawInfo);
			}
		}

		PlayerDrawLayers.DrawPlayer_MakeIntoFirstFractalAfterImage(ref drawInfo);

		PlayerDrawLayers.DrawPlayer_TransformDrawData(ref drawInfo);
		if (scale != 1f)
			PlayerDrawLayers.DrawPlayer_ScaleDrawData(ref drawInfo, scale);

		// Don't render! Just return the data.
		//PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawInfo);
	}
}