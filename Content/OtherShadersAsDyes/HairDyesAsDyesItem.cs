using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SnekVanity.Content.OtherShadersAsDyes;

public sealed class HairDyesAsDyesItem : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return entity.hairDye > 0; // 0 is a valid hairDye value, but it just corresponds to no hair dye (used by Hair Dye Remover).
	}

	public override void SetDefaults(Item item)
	{
		item.dye = PlayerDrawHelper.PackShader(item.hairDye, PlayerDrawHelper.ShaderConfiguration.HairShader);
		item.StatsModifiedBy.Add(Mod);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, $"{nameof(SnekVanity)}:{nameof(HairDyesAsDyesItem)}", Language.GetTextValue($"Mods.{nameof(SnekVanity)}.ExtraTooltip.{nameof(HairDyesAsDyesItem)}")));
	}

	public override void ModifyResearchSorting(Item item, ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
	{
		// Because Item::dye is set, hair dyes get categorized as dyes. Fix that.
		itemGroup = ContentSamples.CreativeHelper.ItemGroup.HairDye;
	}
}