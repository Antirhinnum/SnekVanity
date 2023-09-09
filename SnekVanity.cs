using SnekVanity.Common.Systems;
using Terraria.ModLoader;

namespace SnekVanity;

public sealed class SnekVanity : Mod
{
	public override object Call(params object[] args)
	{
		return CallHandler.Call(args);
	}
}