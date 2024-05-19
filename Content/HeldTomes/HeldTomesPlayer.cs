using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.PlayerEquips;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.HeldTomes;

public sealed class HeldTomesPlayer : ModPlayer, IAddEquipSlots, IAddDyeSlots
{
	private const string _tomeAssetPathHead = "SnekVanity/Assets/Textures/HeldTome_";

	internal static Dictionary<int, Asset<Texture2D>> _knownTomesToAssets;

	public Item heldTomeBack;
	public int cTomeBack;

	public override void Load()
	{
		_knownTomesToAssets = new()
		{
			{ ItemID.Book, null },
			{ ItemID.WaterBolt, null },
			{ ItemID.DemonScythe, null },
			{ ItemID.CrystalStorm, null },
			{ ItemID.CursedFlames, null },
			{ ItemID.SpellTome, null },
			{ ItemID.MagnetSphere, null },
			{ ItemID.BookofSkulls, null },
			{ ItemID.GoldenShower, null },
			{ ItemID.RazorbladeTyphoon, null },
			{ ItemID.LunarFlareBook, null }
		};
	}

	public override void Unload()
	{
		_knownTomesToAssets?.Clear();
		_knownTomesToAssets = null;
	}

	public override void SetStaticDefaults()
	{
		foreach (int tomeType in _knownTomesToAssets.Keys)
		{
			if (ModContent.RequestIfExists(_tomeAssetPathHead + ContentSamples.ItemPersistentIdsByNetIds[tomeType], out Asset<Texture2D> asset))
			{
				_knownTomesToAssets[tomeType] = asset;
			}

			AsymmetricEquipsSystem.AddSpecialItem(tomeType, AsymmetricEquipsSystem.LEFT_SIDE);
		}
	}

	public void ResetVisibleAccessories()
	{
		heldTomeBack = null;
	}

	public void ClearDyeSlots()
	{
		cTomeBack = 0;
	}

	public void UpdateEquipSlot(Item item)
	{
		if (!IsItemTypeHeldTome(item.type))
		{
			return;
		}

		AsymmetricEquipsSystem.GetSideInfo(item, Player, out bool notAsymmetric, out bool correctSide, out _);
		if (notAsymmetric || correctSide)
		{
			heldTomeBack = item;
		}
	}

	public void UpdateDyeSlots(Item armorItem, Item dyeItem)
	{
		if (!IsItemTypeHeldTome(armorItem.type))
		{
			return;
		}

		AsymmetricEquipsSystem.GetSideInfo(armorItem, Player, out bool notAsymmetric, out bool correctSide, out _);
		if (notAsymmetric || correctSide)
		{
			cTomeBack = dyeItem.dye;
		}
	}

	internal static bool IsItemTypeHeldTome(int itemType) => _knownTomesToAssets.ContainsKey(itemType);

	internal bool PlayerIsJumping => Player.bodyFrame.Y / Player.bodyFrame.Height == 5;
	internal bool ShouldDrawTome => !PlayerIsJumping && !Player.compositeBackArm.enabled && Player.balloon <= 0 && Player.balloonFront <= 0 && heldTomeBack != null && _knownTomesToAssets.TryGetValue(heldTomeBack.type, out var asset) && asset != null;

	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		if (ShouldDrawTome)
		{
			// When standing still or using an item, make the player's arm stick out a bit to hold the tome.
			int bodyFrameIndex = Player.bodyFrame.Y / Player.bodyFrame.Height;
			if (bodyFrameIndex < 5)
			{
				drawInfo.compBackArmFrame = TextureAssets.Players[Player.skinVariant, PlayerTextureID.ArmSkin].Frame(9, 4, 4, 3);
			}
		}
	}
}