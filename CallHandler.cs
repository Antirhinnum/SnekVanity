using SnekVanity.Content.DyePlayerTextures;
using Terraria;

namespace SnekVanity;

public static class CallHandler
{
	internal static object Call(object[] args)
	{
		switch (args)
		{
			case ["PlayerBodyDye", Player player, int slot]:
			{
				return player.GetModPlayer<BodyPartDyePlayer>().DyeForPlayerTextureID(slot);
			}
		}

		return null;
	}
}