using SnekVanity.Common.Players;
using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.GlobalItems;

public sealed class ShoulderBirdItem : GlobalItem
{
	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return ShoulderBirdPlayer.IsEquippableBird(entity);
	}

	public override void SetDefaults(Item entity)
	{
		entity.accessory = true;
		entity.vanity = true;
		entity.StatsModifiedBy.Add(Mod);
	}
}