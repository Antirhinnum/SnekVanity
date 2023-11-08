using SnekVanity.Common.Hooks;
using SnekVanity.Common.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Common.Players;

public sealed class ShoulderBirdPlayer : ModPlayer, IAddEquipSlots
{
	private static readonly Dictionary<int, int> _registeredBirds = new()
	{
		{ ItemID.ScarletMacaw, NPCID.ScarletMacaw },
		{ ItemID.BlueMacaw, NPCID.BlueMacaw },
		{ ItemID.Toucan, NPCID.Toucan },
		{ ItemID.GrayCockatiel, NPCID.GrayCockatiel },
		{ ItemID.YellowCockatiel, NPCID.YellowCockatiel }
	};

	public int birdNpcId;
	public int birdFrontNpcId;
	public int cBird;
	public int cBirdFront;

	public override void SetStaticDefaults()
	{
		foreach (int itemId in _registeredBirds.Keys)
		{
			CrossModSystem.AsymmetricEquips_AddSpecialItem(itemId, CrossModSystem.LEFT_SIDE);
		}
	}

	public void ResetVisibleAccessories()
	{
		birdNpcId = -1;
		birdFrontNpcId = -1;
	}

	public void ClearDyeSlots()
	{
		cBird = 0;
		cBirdFront = 0;
	}

	public void UpdateDyeSlots(Item armorItem, Item dyeItem)
	{
		if (!_registeredBirds.ContainsKey(armorItem.type))
		{
			return;
		}

		int dye = dyeItem.dye;
		bool onCorrectSide = CrossModSystem.AsymmetricEquips_ItemOnDefaultSide(armorItem, Player);
		Player.direction = -Player.direction;
		bool onWrongSide = CrossModSystem.AsymmetricEquips_ItemOnDefaultSide(armorItem, Player);
		Player.direction = -Player.direction;
		bool notAsymmetric = onCorrectSide && onWrongSide;

		if (notAsymmetric)
		{
			if (birdFrontNpcId == -1)
			{
				cBirdFront = dye;
			}
			else
			{
				cBird = dye;
			}
		}
		else
		{
			if (onCorrectSide) // BACK SIDE: Bird is on left side when facing right or on right side when facing left
			{
				cBird = dye;
			}
			else // FRONT SIDE: Bird is on right when facing right or on left when facing left
			{
				cBirdFront = dye;
			}
		}
	}

	public void UpdateEquipSlot(Item item)
	{
		if (_registeredBirds.ContainsKey(item.type))
		{
			EquipBird(item);
		}
	}

	public void EquipBird(Item item)
	{
		bool onCorrectSide = CrossModSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player);
		Player.direction = -Player.direction;
		bool onWrongSide = CrossModSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player);
		Player.direction = -Player.direction;
		bool notAsymmetric = onCorrectSide && onWrongSide;

		if (notAsymmetric)
		{
			if (birdFrontNpcId == -1)
			{
				birdFrontNpcId = _registeredBirds[item.type];
			}
			else
			{
				birdNpcId = _registeredBirds[item.type];
			}
		}
		else
		{
			if (onCorrectSide) // BACK SIDE: Bird is on left side when facing right or on right side when facing left
			{
				birdNpcId = _registeredBirds[item.type];
			}
			else // FRONT SIDE: Bird is on right when facing right or on left when facing left
			{
				birdFrontNpcId = _registeredBirds[item.type];
			}
		}
	}

	public static bool IsEquippableBird(Item item)
	{
		return _registeredBirds.ContainsKey(item.type);
	}
}