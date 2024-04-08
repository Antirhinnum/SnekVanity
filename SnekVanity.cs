using SnekVanity.Common.Systems;
using SnekVanity.Content.Items;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SnekVanity;

public sealed class SnekVanity : Mod
{
	private static bool _superEarlyLoaded = false;

	private static TooltipLine _canBeWornTooltipLine;
	internal static TooltipLine CanBeWornTooltipLine
	{
		get
		{
			_canBeWornTooltipLine ??= new(ModContent.GetInstance<SnekVanity>(), "CanBeWorn", Language.GetTextValue($"Mods.{nameof(SnekVanity)}.ExtraTooltip.CanBeWorn"));
			return _canBeWornTooltipLine;
		}
	}

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
		_canBeWornTooltipLine = null;
	}

	public override object Call(params object[] args)
	{
		return CallHandler.Call(args);
	}
}