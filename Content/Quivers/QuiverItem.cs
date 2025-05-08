using SnekVanity.Common;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Quivers;

public sealed class QuiverItem : GlobalItem
{
	internal record struct BackSlotInfo(int VanillaEquipSlot, int ModdedEquipSlot);

	private const string _quiverAssetPathHead = "SnekVanity/Assets/Textures/Quiver_";

	// Values initialized to -1 since the list of keys is needed for registering the equip textures.
	internal static Dictionary<int, BackSlotInfo> QuiverItemTypeToBackEquipId { get; } = new()
	{
		{ ItemID.EndlessQuiver, new(-1, -1) },
		{ ItemID.MagicQuiver, new(ArmorIDs.Back.MagicQuiver, -1) },
		{ ItemID.MoltenQuiver, new(ArmorIDs.Back.MoltenQuiver, -1) },
		{ ItemID.StalkersQuiver, new(ArmorIDs.Back.StalkersQuiver, -1) }
	};

	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return QuiverItemTypeToBackEquipId.ContainsKey(entity.type);
	}

	public override void Load()
	{
		ClientConfig config = ModContent.GetInstance<ClientConfig>();

		foreach (int quiverItemType in QuiverItemTypeToBackEquipId.Keys)
		{
			BackSlotInfo info = QuiverItemTypeToBackEquipId[quiverItemType];
			if (info.VanillaEquipSlot != -1 && !config.UseRespritedQuivers)
			{
				continue;
			}

			if (quiverItemType >= ItemID.Count)
			{
				continue;
			}

			string name = ItemID.Search.GetName(quiverItemType);
			int moddedEquipSlot = EquipLoader.AddEquipTexture(Mod, _quiverAssetPathHead + name, EquipType.Back, name: "Quiver_" + name);
			QuiverItemTypeToBackEquipId[quiverItemType] = info with { ModdedEquipSlot = moddedEquipSlot };
		}
	}

	public override void SetStaticDefaults()
	{
		foreach (BackSlotInfo info in QuiverItemTypeToBackEquipId.Values)
		{
			if (info.ModdedEquipSlot != -1)
			{
				ArmorIDs.Back.Sets.DrawInBackpackLayer[info.ModdedEquipSlot] = true;
			}
		}
	}

	public override void SetDefaults(Item entity)
	{
		// Assume these are already set on modded items.
		if (entity.type >= ItemID.Count)
		{
			return;
		}

		if (QuiverItemTypeToBackEquipId.TryGetValue(entity.type, out BackSlotInfo info))
		{
			if (info.ModdedEquipSlot != -1)
			{
				entity.backSlot = info.ModdedEquipSlot;

				// Don't mark the existing quiver accessories as vanity accessories.
				if (!entity.accessory)
				{
					entity.accessory = true;
					entity.vanity = true;
				}
				entity.StatsModifiedBy.Add(Mod);
			}
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

	internal static void AddQuiverItem(Item item) => QuiverItemTypeToBackEquipId[item.type] = new(item.backSlot, -1);
}