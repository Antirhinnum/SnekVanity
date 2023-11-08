using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SnekVanity.Common.GlobalItems;

/// <summary>
/// Allows all paints to be equipped as dyes.
/// </summary>
public sealed class PaintsAsDyesItem : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return entity.paint > 0;
	}

	public override void SetDefaults(Item item)
	{
		item.dye = PlayerDrawHelper.PackShader(item.paint, PlayerDrawHelper.ShaderConfiguration.TilePaintID);
		item.StatsModifiedBy.Add(Mod);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, $"{nameof(SnekVanity)}:{nameof(PaintsAsDyesItem)}", Language.GetTextValue($"Mods.{nameof(SnekVanity)}.ExtraTooltip.{nameof(PaintsAsDyesItem)}")));
	}
}