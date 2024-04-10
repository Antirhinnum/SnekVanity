using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.PlayerEquips;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.ShoulderBirds;

public sealed class ShoulderBirdPlayer : ModPlayer, IAddEquipSlots, IAddDyeSlots
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

	// Asymmetric Equipment needs to call UpdateDyes() a second time so it works properly.
	// Unfortunately, shoudler bird dyes explode when this happens since they need to know
	//    which birds are equipped in order to choose which dye slot to use.
	// As such, make sure to only reset dyes once!!!
	private bool _alreadyClearedDyesThisFrame = false;

	public override void SetStaticDefaults()
	{
		foreach (int itemId in _registeredBirds.Keys)
		{
			AsymmetricEquipsSystem.AsymmetricEquips_AddSpecialItem(itemId, AsymmetricEquipsSystem.LEFT_SIDE);
		}
	}

	public void ResetVisibleAccessories()
	{
		birdNpcId = -1;
		birdFrontNpcId = -1;
	}

	public void ClearDyeSlots()
	{
		if (!_alreadyClearedDyesThisFrame)
		{
			_alreadyClearedDyesThisFrame = true;
			cBird = 0;
			cBirdFront = 0;
		}
	}

	public override void ResetEffects()
	{
		_alreadyClearedDyesThisFrame = false;
	}

	public void UpdateDyeSlots(Item armorItem, Item dyeItem)
	{
		if (!_registeredBirds.ContainsKey(armorItem.type))
		{
			return;
		}

		int dye = dyeItem.dye;
		bool onCorrectSide = AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(armorItem, Player);
		Player.direction = -Player.direction;
		bool onWrongSide = AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(armorItem, Player);
		Player.direction = -Player.direction;
		bool notAsymmetric = onCorrectSide && onWrongSide;

		if (notAsymmetric)
		{
			if (birdFrontNpcId != -1 && cBirdFront == 0)
			{
				cBirdFront = dye;
			}
			else if (birdNpcId != -1)
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
		if (!_registeredBirds.ContainsKey(item.type))
		{
			return;
		}

		bool onCorrectSide = AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player);
		Player.direction = -Player.direction;
		bool onWrongSide = AsymmetricEquipsSystem.AsymmetricEquips_ItemOnDefaultSide(item, Player);
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