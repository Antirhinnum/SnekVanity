using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.OtherShadersAsDyes;

public sealed class HairDyePlayer : ModPlayer
{
	public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
	{
		// Vanilla has some strange condition that applies Life Hair Dye by default???
		// Revert the hair dye back to undyed if we think this is the case.
		if (Player.head == ArmorIDs.Head.FamiliarWig
			&& Player.hairDye == 0
			&& drawInfo.hairDyePacked == PlayerDrawHelper.PackShader(1, PlayerDrawHelper.ShaderConfiguration.HairShader))
		{
			drawInfo.hairDyePacked = PlayerDrawHelper.PackShader(0, PlayerDrawHelper.ShaderConfiguration.HairShader);
		}
	}
}