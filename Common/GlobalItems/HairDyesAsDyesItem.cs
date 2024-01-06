using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SnekVanity.Common.GlobalItems;

public sealed class HairDyesAsDyesItem : GlobalItem
{
	public sealed class HairDyePlayer : ModPlayer
	{
		public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
		{
			// Vanilla has some strange condition that applies Life Hair Dye by default???
			if (Player.head == ArmorIDs.Head.FamiliarWig && Player.hairDye == 0)
			{
				drawInfo.hairDyePacked = PlayerDrawHelper.PackShader(0, PlayerDrawHelper.ShaderConfiguration.HairShader);
			}
		}
	}

	public sealed class HairDyeColorLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition()
		{
			return PlayerDrawLayers.AfterLastVanillaLayer;
		}

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			for (int i = 0; i < drawInfo.DrawDataCache.Count; i++)
			{
				DrawData data = drawInfo.DrawDataCache[i];
				PlayerDrawHelper.UnpackShader(data.shader, out int localShaderIndex, out PlayerDrawHelper.ShaderConfiguration shaderType);
				if (shaderType == PlayerDrawHelper.ShaderConfiguration.HairShader && localShaderIndex != 0)
				{
					data.color = GameShaders.Hair.GetColor(localShaderIndex, drawInfo.drawPlayer, data.color);
					drawInfo.DrawDataCache[i] = data;
				}
			}
		}
	}

	public override bool AppliesToEntity(Item entity, bool lateInstantiation)
	{
		return entity.hairDye > 0; // 0 is a valid hairDye value, but it just corresponds to no hair dye (used by Hair Dye Remover).
	}

	public override void SetDefaults(Item item)
	{
		item.dye = PlayerDrawHelper.PackShader(item.hairDye, PlayerDrawHelper.ShaderConfiguration.HairShader);
		item.StatsModifiedBy.Add(Mod);
	}

	public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
	{
		tooltips.Add(new TooltipLine(Mod, $"{nameof(SnekVanity)}:{nameof(HairDyesAsDyesItem)}", Language.GetTextValue($"Mods.{nameof(SnekVanity)}.ExtraTooltip.{nameof(HairDyesAsDyesItem)}")));
	}

	public override void ModifyResearchSorting(Item item, ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
	{
		// Because Item::dye is set, hair dyes get categorized as dyes. Fix that.
		itemGroup = ContentSamples.CreativeHelper.ItemGroup.HairDye;
	}
}