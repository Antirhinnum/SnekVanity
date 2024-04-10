using SnekVanity.Common.PlayerEquips;
using System;
using Terraria;
using Terraria.GameContent.Golf;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Umbrellas;

// Hijacks the drawing from the mod "Equippable Umbrellas"/"Parasol".
public sealed class ParasolHijackPlayer : ModPlayer, IAddEquipSlots, IAddDyeSlots
{
	private const string _parasolModName = "Parasol";

	public Item equippedUmbrella;
	public int cUmbrella;
	private bool _wearingUmbrellaHandledByParasol;

	public bool HasEquippedUmbrella => equippedUmbrella != null && !equippedUmbrella.IsAir;
	public bool HasEquippedUmbrellaAndNotWearingParasolHandledUmbrella => HasEquippedUmbrella && !_wearingUmbrellaHandledByParasol;

	public override bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.HasMod(_parasolModName);
	}

	public void ResetVisibleAccessories()
	{
		equippedUmbrella = null;
		_wearingUmbrellaHandledByParasol = false;
	}

	public void ClearDyeSlots()
	{
		cUmbrella = 0;
	}

	private bool IsItemUmbrellaAndCheckForVanilla(Item item)
	{
		if (item.type == ItemID.Umbrella || item.type == ItemID.TragicUmbrella)
		{
			_wearingUmbrellaHandledByParasol = true;
			return true;
		}

		return item.ModItem is IEquippableParasol;
	}

	public void UpdateEquipSlot(Item item)
	{
		if (IsItemUmbrellaAndCheckForVanilla(item))
		{
			equippedUmbrella = item;

			if (!HasEquippedUmbrellaAndNotWearingParasolHandledUmbrella || Player.HeldItem.type == ItemID.FairyQueenMagicItem || GolfHelper.IsPlayerHoldingClub(Player) || Player.HeldItem.holdStyle == ItemHoldStyleID.HoldGuitar)
			{
				return;
			}

			bool holdingUp = Player.slowFall && !Player.controlDown && Player.velocity.Y > 0f;
			float turnArmAmt = MathF.PI * -(holdingUp ? 3f : 1f) / 5f;
			Player.SetCompositeArmBack(enabled: true, Player.CompositeArmStretchAmount.ThreeQuarters, turnArmAmt * Player.direction);
		}
	}

	public void UpdateDyeSlots(Item armorItem, Item dyeItem)
	{
		if (IsItemUmbrellaAndCheckForVanilla(armorItem))
		{
			cUmbrella = dyeItem.dye;
		}
	}

	public sealed class ParasolItem : GlobalItem
	{
		public override bool IsLoadingEnabled(Mod mod)
		{
			return ModLoader.HasMod(_parasolModName);
		}

		public override bool AppliesToEntity(Item entity, bool lateInstantiation)
		{
			return entity.ModItem is IEquippableParasol;
		}

		public override void SetDefaults(Item entity)
		{
			entity.DamageType = DamageClass.Default;
			entity.damage = 0;
			entity.useStyle = ItemUseStyleID.None;
			entity.holdStyle = ItemHoldStyleID.None;
			entity.accessory = true;
		}

		public override void UpdateAccessory(Item item, Player player, bool hideVisual)
		{
			if ((item.ModItem as IEquippableParasol).ActivatesSlowFall(player))
			{
				player.slowFall = true;
			}
		}
	}
}