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
			DrawData data = drawInfo.DrawDataCache[i];
			bool playerSkinData = _frontArmPlayerTextureIds.Any(i => PlayerDrawHelpers.UsesPlayerTexture(data, drawPlayer, i));
			bool bodyData = drawPlayer.body > -1 && (data.texture == TextureAssets.ArmorBody[drawPlayer.body].Value || data.texture == TextureAssets.ArmorBodyComposite[drawPlayer.body].Value || data.texture == TextureAssets.FemaleBody[drawPlayer.body].Value);

			if (playerSkinData || bodyData)
			{
				if (hiddenPlayer.hideFrontArm && data.sourceRect == drawInfo.compFrontArmFrame)
				{
					drawInfo.DrawDataCache.RemoveAt(i);
					i--;
				}
				else if (hiddenPlayer.hideBackArm && data.sourceRect == drawInfo.compBackArmFrame)
				{
					drawInfo.DrawDataCache.RemoveAt(i);
					i--;
				}
			}
		}
	}
}