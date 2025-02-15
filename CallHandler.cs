using SnekVanity.Common;
using SnekVanity.Content.DyePlayerTextures;
using Terraria;
using Terraria.ModLoader;

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
			case ["GetConfigOption", string name]:
			{
				ClientConfig config = ModContent.GetInstance<ClientConfig>();
				return name switch
				{
					"ShowStavesWhenHeld" => config.ShowStavesWhenHeld,
					"ShowTomesWhenHeld" => config.ShowTomesWhenHeld,
					"ShowSheathsWhenHeld" => config.ShowSheathsWhenHeld,
					_ => null
				};
			}
		}

		return null;
	}
}