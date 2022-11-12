using SnekVanity.Common.Systems;
using SnekVanity.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace SnekVanity.Common.Players;

public sealed class DyePlayer : ModPlayer
{
	/// <summary>
	/// The shader index applied to this player's head.
	/// </summary>
	public int cHeadSkin;

	/// <summary>
	/// The shader index applied to this player's scleras.
	/// </summary>
	public int cEyeWhites;

	/// <summary>
	/// The shader index applied to this player's pupils.
	/// </summary>
	public int cEye;

	/// <summary>
	/// The shader index applied to this player's body.
	/// </summary>
	public int cTorsoSkin;

	/// <summary>
	/// The shader index applied to this player's undershirt.
	/// </summary>
	public int cUndershirt;

	/// <summary>
	/// The shader index applied to this player's hands.
	/// </summary>
	public int cHandSkin;

	/// <summary>
	/// The shader index applied to this player's shirt.
	/// </summary>
	public int cShirt;

	/// <summary>
	/// The shader index applied to this player's arms.
	/// </summary>
	public int cArmSkin;

	/// <summary>
	/// The shader index applied to this player's legs.
	/// </summary>
	public int cLegSkin;

	/// <summary>
	/// The shader index applied to this player's pants.
	/// </summary>
	public int cPants;

	/// <summary>
	/// The shader index applied to this player's shoes.
	/// </summary>
	public int cShoes;

	/// <summary>
	/// The shader index applied to this player's eyelids.
	/// </summary>
	public int cEyeBlink;

	public override void Load()
	{
		On.Terraria.Player.UpdateDyes += ResetCustomDyes;
		On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_TransformDrawData += ModifyPlayerDyes;
	}

	public override void Unload()
	{
		On.Terraria.Player.UpdateDyes -= ResetCustomDyes;
		On.Terraria.DataStructures.PlayerDrawLayers.DrawPlayer_TransformDrawData -= ModifyPlayerDyes;
	}

	/// <summary>
	/// Reset the player's dyes alongside vanilla.
	/// </summary>
	/// <param name="orig"></param>
	/// <param name="self"></param>
	private static void ResetCustomDyes(On.Terraria.Player.orig_UpdateDyes orig, Player self)
	{
		if (self.TryGetModPlayer(out DyePlayer dPlayer))
		{
			dPlayer.cHeadSkin = 0;
			dPlayer.cEyeWhites = 0;
			dPlayer.cEye = 0;
			dPlayer.cTorsoSkin = 0;
			dPlayer.cUndershirt = 0;
			dPlayer.cHandSkin = 0;
			dPlayer.cShirt = 0;
			dPlayer.cArmSkin = 0;
			dPlayer.cLegSkin = 0;
			dPlayer.cPants = 0;
			dPlayer.cShoes = 0;
			dPlayer.cEyeBlink = 0;
		}

		orig(self);
	}

	/// <summary>
	/// A recreation of <see cref="Player.UpdateDyes"/> that only updates body dyes.
	/// Without this, dyes are off by one frame when asymmetric.
	/// </summary>
	private static void UpdatePlayerBodyDyes(Player player)
	{
		for (int i = 0; i < player.armor.Length; i++)
		{
			if (player.IsAValidEquipmentSlotForIteration(i))
			{
				UpdatePlayerBodyDye(player, i < 10, player.hideVisibleAccessory[i % 10], player.armor[i], player.dye[i % 10]);
			}
		}

		if (player.TryGetModPlayer(out ModAccessorySlotPlayer mPlayer))
		{
			int slots = mPlayer.SlotCount;
			AccessorySlotLoader loader = LoaderManager.Get<AccessorySlotLoader>();
			for (int i = 0; i < slots; i++)
			{
				if (loader.ModdedIsAValidEquipmentSlotForIteration(i, player))
				{
					ModAccessorySlot slot = loader.Get(i);
					UpdatePlayerBodyDye(player, true, slot.HideVisuals, slot.FunctionalItem, slot.DyeItem);
					UpdatePlayerBodyDye(player, false, slot.HideVisuals, slot.VanityItem, slot.DyeItem);
				}
			}
		}
	}

	/// <summary>
	/// Updates the player's body dyes from the given item.
	/// </summary>
	private static void UpdatePlayerBodyDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
	{
		if ((isSetToHidden && isNotInVanitySlot) || !player.TryGetModPlayer(out DyePlayer dPlayer) || armorItem.ModItem is not ModItem modItem)
		{
			return;
		}

		if (!CrossModSystem.AsymmetricEquips_ItemOnFrontSide(armorItem, player))
		{
			return;
		}

		int dye = dyeItem.dye;

		if (modItem is IDyeHeadSkin)
		{
			dPlayer.cHeadSkin = dye;
		}

		if (modItem is IDyeEyeWhites)
		{
			dPlayer.cEyeWhites = dye;
		}

		if (modItem is IDyeEyes)
		{
			dPlayer.cEye = dye;
		}

		if (modItem is IDyeTorsoSkin)
		{
			dPlayer.cTorsoSkin = dye;
		}

		if (modItem is IDyeUndershirt)
		{
			dPlayer.cUndershirt = dye;
		}

		if (modItem is IDyeHandSkin)
		{
			dPlayer.cHandSkin = dye;
		}

		if (modItem is IDyeShirt)
		{
			dPlayer.cShirt = dye;
		}

		if (modItem is IDyeArmSkin)
		{
			dPlayer.cArmSkin = dye;
		}

		if (modItem is IDyeLegSkin)
		{
			dPlayer.cLegSkin = dye;
		}

		if (modItem is IDyePants)
		{
			dPlayer.cPants = dye;
		}

		if (modItem is IDyeShoes)
		{
			dPlayer.cShoes = dye;
		}

		if (modItem is IDyeEyeBlink)
		{
			dPlayer.cEyeBlink = dye;
		}
	}

	/// <summary>
	/// Applies the player's body dyes to their DrawData right before the player is drawn.
	/// </summary>
	private static void ModifyPlayerDyes(On.Terraria.DataStructures.PlayerDrawLayers.orig_DrawPlayer_TransformDrawData orig, ref PlayerDrawSet drawinfo)
	{
		orig(ref drawinfo);

		// Change the shader of each layer we'd like to dye.
		Player player = drawinfo.drawPlayer;
		if (!player.TryGetModPlayer(out DyePlayer dPlayer))
		{
			return;
		}

		UpdatePlayerBodyDyes(player);

		// Since assets are stored in one place, any body assets that show up in DrawDataCache will be references in TextureAssets.
		static bool IsCorrectTexture(DrawData data, Player player, int playerTextureID) => data.texture == TextureAssets.Players[player.skinVariant, playerTextureID].Value;

		for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
		{
			DrawData data = drawinfo.DrawDataCache[i];

			for (int j = 0; j < PlayerTextureID.Count; j++)
			{
				int shader = dPlayer.DyeForPlayerTextureID(j);
				if (IsCorrectTexture(data, player, j) && shader > 0)
				{
					data.shader = shader;
					drawinfo.DrawDataCache[i] = data;
				}
			}
		}
	}

	/// <summary>
	/// Gets the appropriate dye field for the given <see cref="PlayerTextureID"/>.
	/// </summary>
	/// <param name="playerTextureID">The <see cref="PlayerTextureID"/> to get the dye for.</param>
	/// <returns>The appropriate dye if <paramref name="playerTextureID"/> is valid, and 0 otherwise.</returns>
	public int DyeForPlayerTextureID(int playerTextureID)
	{
		return playerTextureID switch
		{
			PlayerTextureID.Head => cHeadSkin,
			PlayerTextureID.EyeWhites => cEyeWhites,
			PlayerTextureID.Eyes => cEye,
			PlayerTextureID.TorsoSkin => cTorsoSkin,
			PlayerTextureID.Undershirt or PlayerTextureID.ArmUndershirt => cUndershirt,
			PlayerTextureID.Hands or PlayerTextureID.ArmHand => cHandSkin,
			PlayerTextureID.Shirt or PlayerTextureID.ArmShirt or PlayerTextureID.Extra => cShirt,
			PlayerTextureID.ArmSkin => cArmSkin,
			PlayerTextureID.LegSkin => cLegSkin,
			PlayerTextureID.Pants => cPants,
			PlayerTextureID.Shoes => cShoes,
			PlayerTextureID.EyeBlink => cEyeBlink,
			_ => 0
		};
	}
}