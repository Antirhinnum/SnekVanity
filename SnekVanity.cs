using SnekVanity.Common.Systems;
using SnekVanity.Content.Items;
using Terraria.ModLoader;

namespace SnekVanity;

public sealed class SnekVanity : Mod
{
	private static bool _superEarlyLoaded = false;

	public SnekVanity()
	{
		if (_superEarlyLoaded)
		{
			return;
		}

		_superEarlyLoaded = true;

		CombinedDyeItem.DoSuperEarlyHooks();
	}

	public override void Unload()
	{
		_superEarlyLoaded = false;
	}

	public override object Call(params object[] args)
	{
		return CallHandler.Call(args);
	}
}