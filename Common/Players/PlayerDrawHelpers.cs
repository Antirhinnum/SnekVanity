using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;

namespace SnekVanity.Common.Players;

public static class PlayerDrawHelpers
{
	public static bool UsesPlayerTexture(DrawData data, Player player, int playerTextureID) => data.texture == TextureAssets.Players[player.skinVariant, playerTextureID].Value;
}