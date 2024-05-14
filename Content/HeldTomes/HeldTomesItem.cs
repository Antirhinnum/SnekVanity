using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.HeldTomes;

public sealed class HeldTomesItem : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return HeldTomesPlayer.IsItemTypeHeldTome(entity.type);
	}

	public override void SetDefaults(Item entity)
	{
		// Assume these are already set on modded items.
		if (entity.type >= ItemID.Count)
		{
			return;
		}

		entity.accessory = true;
		entity.vanity = true;
		entity.StatsModifiedBy.Add(Mod);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (item.damage > 0)
		{
			tooltips.Add(SnekVanity.CanBeWornTooltipLine);
		}
	}
}