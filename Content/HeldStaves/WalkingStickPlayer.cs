using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Content.HeldStaves;

public sealed class WalkingStickPlayer : ModPlayer
{
	public bool hasWalkingStick;

	public override void ResetEffects()
	{
		hasWalkingStick = false;
	}

	public override void PostUpdateRunSpeeds()
	{
		if (hasWalkingStick)
		{
			Player.runAcceleration *= 1.05f;
			Player.runSlowdown *= 1.05f;
		}
	}
}