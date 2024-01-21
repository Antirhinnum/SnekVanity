using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SnekVanity.Common.Players;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.DrawLayers;

public sealed class HideBodyPartsLayer : PlayerDrawLayer
{
	private static readonly int[] _frontArmPlayerTextureIds = { PlayerTextureID.ArmUndershirt, PlayerTextureID.ArmShirt, PlayerTextureID.Hands, PlayerTextureID.ArmSkin, PlayerTextureID.ArmHand };

	public override Position GetDefaultPosition()
	{
		return new AfterParent(PlayerDrawLayers.HandOnAcc);
	}

	public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
	{
		return drawInfo.drawPlayer.GetModPlayer<HiddenBodyPartsPlayer>().AnyHiddenBodyParts;
	}

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		Player drawPlayer = drawInfo.drawPlayer;
		HiddenBodyPartsPlayer hiddenPlayer = drawPlayer.GetModPlayer<HiddenBodyPartsPlayer>();

		for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
		{
			static bool AreTexturesSame(Asset<Texture2D>[] array, Texture2D toCompare, int index) => array != null && array.IndexInRange(index) && array[index] != null && array[index].Value == toCompare;

			DrawData data = drawInfo.DrawDataCache[i];
			bool playerSkinData = _frontArmPlayerTextureIds.Any(i => PlayerDrawHelpers.UsesPlayerTexture(data, drawPlayer, i));
			bool isBodyTexture = drawPlayer.body > -1 && (AreTexturesSame(TextureAssets.ArmorBodyComposite, data.texture, drawPlayer.body) || AreTexturesSame(TextureAssets.ArmorBody, data.texture, drawPlayer.body) || AreTexturesSame(TextureAssets.FemaleBody, data.texture, drawPlayer.body));
			bool isFrontArmFrame = hiddenPlayer.hideFrontArm && data.sourceRect == drawInfo.compFrontArmFrame;
			bool isBackArmFrame = hiddenPlayer.hideBackArm && data.sourceRect == drawInfo.compBackArmFrame;

			if ((playerSkinData || isBodyTexture) && (isFrontArmFrame || isBackArmFrame))
			{
				if (drawInfo.projectileDrawPosition > i)
				{
					drawInfo.projectileDrawPosition--;
				}

				drawInfo.DrawDataCache.RemoveAt(i);
				i--;
			}
		}
	}
}