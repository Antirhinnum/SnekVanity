using SnekVanity.Common.Hooks;
using SnekVanity.Common.Systems;
using SnekVanity.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Common.Players;

public sealed class BodyPartDyePlayer : ModPlayer, IAddDyeSlots
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

	/// <summary>
	/// The shader index applied to this player's hair.
	/// </summary>
	public int cHairCustom;

	public override void Load()
	{
		On_PlayerDrawLayers.DrawPlayer_TransformDrawData += ModifyPlayerDyes;
	}

	public void ClearDyeSlots()
	{
		cHeadSkin = 0;
		cEyeWhites = 0;
		cEye = 0;
		cTorsoSkin = 0;
		cUndershirt = 0;
		cHandSkin = 0;
		cShirt = 0;
		cArmSkin = 0;
		cLegSkin = 0;
		cPants = 0;
		cShoes = 0;
		cEyeBlink = 0;
		cHairCustom = 0;
	}

	public void UpdateDyeSlots(Item armorItem, Item dyeItem)
	{
		if (armorItem.ModItem is not ModItem modItem || !CrossModSystem.AsymmetricEquips_ItemOnDefaultSide(armorItem, Player))
		{
			return;
		}

		int dye = dyeItem.dye;

		if (modItem is IDyeHeadSkin)
		{
			cHeadSkin = dye;
		}

		if (modItem is IDyeEyeWhites)
		{
			cEyeWhites = dye;
		}

		if (modItem is IDyeEyes)
		{
			cEye = dye;
		}

		if (modItem is IDyeTorsoSkin)
		{
			cTorsoSkin = dye;
		}

		if (modItem is IDyeUndershirt)
		{
			cUndershirt = dye;
		}

		if (modItem is IDyeHandSkin)
		{
			cHandSkin = dye;
		}

		if (modItem is IDyeShirt)
		{
			cShirt = dye;
		}

		if (modItem is IDyeArmSkin)
		{
			cArmSkin = dye;
		}

		if (modItem is IDyeLegSkin)
		{
			cLegSkin = dye;
		}

		if (modItem is IDyePants)
		{
			cPants = dye;
		}

		if (modItem is IDyeShoes)
		{
			cShoes = dye;
		}

		if (modItem is IDyeEyeBlink)
		{
			cEyeBlink = dye;
		}

		if (modItem is IDyeHair)
		{
			cHairCustom = dye;
		}
	}

	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		if (cHairCustom != 0)
		{
			drawInfo.hairDyePacked = PlayerDrawHelper.PackShader(cHairCustom, PlayerDrawHelper.ShaderConfiguration.ArmorShader);
		}
	}

	/// <summary>
	/// Applies the player's body dyes to their DrawData right before the player is drawn.
	/// <br/> Exists because most of the player's body parts are drawn without providing a shader index that we could overwrite (like with hair).
	/// </summary>
	private static void ModifyPlayerDyes(On_PlayerDrawLayers.orig_DrawPlayer_TransformDrawData orig, ref PlayerDrawSet drawinfo)
	{
		orig(ref drawinfo);

		// Change the shader of each layer we'd like to dye.
		Player player = drawinfo.drawPlayer;
		if (!player.TryGetModPlayer(out BodyPartDyePlayer bodyDyePlayer))
		{
			return;
		}

		for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
		{
			DrawData data = drawinfo.DrawDataCache[i];

			for (int j = 0; j < PlayerTextureID.Count; j++)
			{
				int shader = bodyDyePlayer.DyeForPlayerTextureID(j);
				if (PlayerDrawHelpers.UsesPlayerTexture(data, player, j) && shader > 0)
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