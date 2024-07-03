using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SnekVanity.Common.CrossMod.AsymmetricEquips;
using SnekVanity.Common.PlayerEquips;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Sheaths;

public sealed class SheathPlayer : ModPlayer, IAddEquipSlots, IAddDyeSlots
{
	private const string _sheathAssetPathHead = "SnekVanity/Assets/Textures/Sheath_";

	internal static Dictionary<int, Asset<Texture2D>> KnownSwordsToSheathAssets { get; private set; }
	internal static Dictionary<int, Asset<Texture2D>> KnownSwordsToSheathGlowAssets { get; private set; }

	public Item sheathBack;
	public Item sheathFront;
	public int cSheathBack;
	public int cSheathFront;

	public override void Load()
	{
		KnownSwordsToSheathAssets = new()
		{
			{ ItemID.Katana, null },
			{ ItemID.Muramasa, null },
			{ ItemID.CobaltSword, null },
			{ ItemID.CandyCaneSword, null },
			{ ItemID.RedPhaseblade, null },
			{ ItemID.OrangePhaseblade, null },
			{ ItemID.YellowPhaseblade, null },
			{ ItemID.GreenPhaseblade, null },
			{ ItemID.BluePhaseblade, null },
			{ ItemID.PurplePhaseblade, null },
			{ ItemID.WhitePhaseblade, null },
			{ ItemID.RedPhasesaber, null },
			{ ItemID.OrangePhasesaber, null },
			{ ItemID.YellowPhasesaber, null },
			{ ItemID.GreenPhasesaber, null },
			{ ItemID.BluePhasesaber, null },
			{ ItemID.PurplePhasesaber, null },
			{ ItemID.WhitePhasesaber, null }
		};
		KnownSwordsToSheathGlowAssets = [];
	}

	public override void Unload()
	{
		KnownSwordsToSheathAssets?.Clear();
		KnownSwordsToSheathAssets = null;
		KnownSwordsToSheathGlowAssets?.Clear();
		KnownSwordsToSheathGlowAssets = null;
	}

	public override void SetStaticDefaults()
	{
		foreach (int swordType in KnownSwordsToSheathAssets.Keys)
		{
			string path = _sheathAssetPathHead + ContentSamples.ItemPersistentIdsByNetIds[swordType];
			if (ModContent.RequestIfExists(path, out Asset<Texture2D> asset))
			{
				KnownSwordsToSheathAssets[swordType] = asset;

				if (ModContent.RequestIfExists(path + "_Glow", out Asset<Texture2D> glowAsset))
				{
					KnownSwordsToSheathGlowAssets[swordType] = glowAsset;
				}
			}

			AsymmetricEquipsSystem.AddSpecialItem(swordType, AsymmetricEquipsSystem.LEFT_SIDE);
		}
	}

	public void ResetVisibleAccessories()
	{
		sheathBack = null;
		sheathFront = null;
	}

	public void ClearDyeSlots()
	{
		cSheathBack = 0;
		cSheathFront = 0;
	}

	public void UpdateEquipSlot(Item item)
	{
		if (!DoesItemTypeHaveSheath(item.type))
		{
			return;
		}

		AsymmetricEquipsSystem.GetSideInfo(item, Player, out bool notAsymmetric, out bool correctSide, out bool wrongSide);
		if (notAsymmetric || correctSide)
		{
			sheathBack = item;
		}
		else if (wrongSide)
		{
			sheathFront = item;
		}
	}

	public void UpdateDyeSlots(Item armorItem, Item dyeItem)
	{
		if (!DoesItemTypeHaveSheath(armorItem.type))
		{
			return;
		}

		AsymmetricEquipsSystem.GetSideInfo(armorItem, Player, out bool notAsymmetric, out bool correctSide, out bool wrongSide);
		if (notAsymmetric || correctSide)
		{
			cSheathBack = dyeItem.dye;
		}
		else if (wrongSide)
		{
			cSheathFront = dyeItem.dye;
		}
	}

	internal static bool DoesItemTypeHaveSheath(int itemType) => KnownSwordsToSheathAssets.ContainsKey(itemType);

	internal bool ShouldDrawSheath => (sheathBack != null && KnownSwordsToSheathAssets.TryGetValue(sheathBack.type, out var asset) && asset != null)
		|| (sheathFront != null && KnownSwordsToSheathAssets.TryGetValue(sheathFront.type, out asset) && asset != null);
}