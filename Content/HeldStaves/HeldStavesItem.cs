using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.HeldStaves;

public sealed class HeldStavesItem : GlobalItem
{
	private const string _staveAssetPathHead = "SnekVanity/Assets/Textures/HeldStave_";

	// Values initialized to -1 since the list of keys is needed for registering the equip textures.
	private static readonly Dictionary<int, int> _staveItemTypeToBalloonEquipId = new()
	{
		{ ItemID.AmethystStaff, -1 },
		{ ItemID.TopazStaff, -1 },
		{ ItemID.SapphireStaff, -1 },
		{ ItemID.EmeraldStaff, -1 },
		{ ItemID.RubyStaff, -1 },
		{ ItemID.DiamondStaff, -1 },
		{ ItemID.AmberStaff, -1 },
		{ ItemID.FrostStaff, -1 },
		{ ItemID.ShadowbeamStaff, -1 }
	};

	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return _staveItemTypeToBalloonEquipId.ContainsKey(entity.type);
	}

	public override void Load()
	{
		foreach (int staveItemType in _staveItemTypeToBalloonEquipId.Keys)
		{
			// WARNING: This will break for modded items, as they aren't added to ItemID::Search yet.
			string name = ItemID.Search.GetName(staveItemType);
			_staveItemTypeToBalloonEquipId[staveItemType] = EquipLoader.AddEquipTexture(Mod, _staveAssetPathHead + name, EquipType.Balloon, name: "HeldStave_" + name);
		}
	}

	public override void SetStaticDefaults()
	{
		foreach (int heldStaveBalloonId in _staveItemTypeToBalloonEquipId.Values)
		{
			ArmorIDs.Balloon.Sets.DrawInFrontOfBackArmLayer[heldStaveBalloonId] = true;
			ArmorIDs.Balloon.Sets.UsesTorsoFraming[heldStaveBalloonId] = true;
		}
	}

	public override void SetDefaults(Item entity)
	{
		if (_staveItemTypeToBalloonEquipId.TryGetValue(entity.type, out int slot))
		{
			entity.balloonSlot = slot;
			entity.accessory = true;
			entity.vanity = true;
			entity.StatsModifiedBy.Add(Mod);
		}
		else
		{
			Mod.Logger.DebugFormat("HeldStavesItem::SetDefaults() was called on {0} for some reason?", ItemID.Search.GetName(entity.type));
		}
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		tooltips.Add(SnekVanity.CanBeWornTooltipLine);
	}

	internal static bool IsBalloonIdAHeldStave(int balloonId) => _staveItemTypeToBalloonEquipId.Values.Contains(balloonId);
}