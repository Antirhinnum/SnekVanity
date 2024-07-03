using SnekVanity.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Sheaths;

public sealed class SheathItem : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return SheathPlayer.DoesItemTypeHaveSheath(entity.type);
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
		entity.hasVanityEffects = true;
		entity.StatsModifiedBy.Add(Mod);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (item.damage > 0)
		{
			tooltips.Add(SnekVanity.CanBeWornTooltipLine);
		}
	}

	public override void HoldItem(Item item, Player player)
	{
		if (ModContent.GetInstance<ClientConfig>().ShowSheathsWhenHeld && player.TryGetModPlayer(out SheathPlayer sheathPlayer))
		{
			sheathPlayer.UpdateEquipSlot(item);
		}
	}
}