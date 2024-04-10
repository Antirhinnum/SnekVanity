using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.ForbiddenSigil;

/// <summary>
/// Draws the <see cref="ForbiddenArmorSigilItem"/>.
/// </summary>
public sealed class ForbiddenArmorSigilPlayerLayer : PlayerDrawLayer
{
	public override Position GetDefaultPosition()
	{
		return new AfterParent(PlayerDrawLayers.ForbiddenSetRing);
	}

	protected override void Draw(ref PlayerDrawSet drawInfo)
	{
		Player player = drawInfo.drawPlayer;
		ForbiddenArmorSigilPlayer forbiddenPlayer = player.GetModPlayer<ForbiddenArmorSigilPlayer>();
		if (!forbiddenPlayer.sigilActive)
		{
			return;
		}

		if (!player.setForbidden)
		{
			// The sigil isn't present, so add it.
			int oldCBody = drawInfo.cBody;
			drawInfo.cBody = forbiddenPlayer.cSigil;
			player.setForbidden = true;

			PlayerDrawLayers.DrawPlayer_05_ForbiddenSetRing(ref drawInfo);

			drawInfo.cBody = oldCBody;
			player.setForbidden = false;
		}
		else if (forbiddenPlayer.cSigil > 0)
		{
			// The sigil *is* present, so dye it if needed.
			for (int i = drawInfo.DrawDataCache.Count - 1; i >= 0; i--)
			{
				DrawData data = drawInfo.DrawDataCache[i];
				if (data.texture == TextureAssets.Extra[ExtrasID.ForbiddenSign].Value)
				{
					data.shader = forbiddenPlayer.cSigil;
					drawInfo.DrawDataCache[i] = data;
					break;
				}
			}
		}
	}
}