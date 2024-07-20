using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Quivers;

public sealed class QuiverItem : GlobalItem
{
	private const string _quiverAssetPathHead = "SnekVanity/Assets/Textures/Quiver_";

	// Values initialized to -1 since the list of keys is needed for registering the equip textures.
	internal static Dictionary<int, int> QuiverItemTypeToBackEquipId { get; } = new()
	{
		{ ItemID.EndlessQuiver, -1 },
		{ ItemID.MagicQuiver, -1 },
		{ ItemID.MoltenQuiver, -1 },
		{ ItemID.StalkersQuiver, -1 }
	};

	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return QuiverItemTypeToBackEquipId.ContainsKey(entity.type);
	}

	public override void Load()
	{
		foreach (int quiverItemType in QuiverItemTypeToBackEquipId.Keys)
		{
			if (quiverItemType >= ItemID.Count)
			{
				continue;
			}

			string name = ItemID.Search.GetName(quiverItemType);
			QuiverItemTypeToBackEquipId[quiverItemType] = EquipLoader.AddEquipTexture(Mod, _quiverAssetPathHead + name, EquipType.Back, name: "Quiver_" + name);
		}
	}

	public override void SetStaticDefaults()
	{
		foreach (int quiverBackId in QuiverItemTypeToBackEquipId.Values)
		{
			ArmorIDs.Back.Sets.DrawInBackpackLayer[quiverBackId] = true;
		}
	}

	public override void SetDefaults(Item entity)
	{
		// Assume these are already set on modded items.
		if (entity.type >= ItemID.Count)
		{
			return;
		}

		if (QuiverItemTypeToBackEquipId.TryGetValue(entity.type, out int slot))
		{
			entity.backSlot = slot;

			// Don't mark the existing quiver accessories as vanity accessories.
			if (!entity.accessory)
			{
				entity.accessory = true;
				entity.vanity = true;
			}
			entity.StatsModifiedBy.Add(Mod);
		}
		else
		{
			Mod.Logger.DebugFormat("QuiverItem::SetDefaults() was called on {0} for some reason?", ItemID.Search.GetName(entity.type));
		}
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		if (item.damage > 0)
		{
			tooltips.Add(SnekVanity.CanBeWornTooltipLine);
		}
	}

	internal static void AddQuiverItem(Item item) => QuiverItemTypeToBackEquipId[item.type] = item.backSlot;
}