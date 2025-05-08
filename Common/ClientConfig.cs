using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SnekVanity.Common;

public sealed class ClientConfig : ModConfig
{
	public override ConfigScope Mode { get; } = ConfigScope.ClientSide;

	[DefaultValue(true)]
	public bool ShowStavesWhenHeld;

	[DefaultValue(true)]
	public bool ShowTomesWhenHeld;

	[DefaultValue(true)]
	public bool ShowSheathsWhenHeld;

	[DefaultValue(true)]
	[ReloadRequired]
	public bool UseRespritedQuivers;
}