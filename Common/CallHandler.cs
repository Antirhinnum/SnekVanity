using SnekVanity.Common.Players;
using Terraria;

namespace SnekVanity.Common.Systems;

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