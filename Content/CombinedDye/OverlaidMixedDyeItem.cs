using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.ShopSelling;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.CombinedDye;

[LegacyName("CombinedDyeItem", "OverlaidMixedDye")]
public sealed class OverlaidMixedDyeItem : AMixedDyeItem, IAmSoldByVanillaNPC
{
	[field: CloneByReference]
	Condition IAmSoldByVanillaNPC.Available { get; } = Condition.Hardmode;

	int IAmSoldByVanillaNPC.NPC { get; } = NPCID.WitchDoctor;

	protected override bool CanAcceptUselessDye => false;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<VerticalGradientMixedDyeItem>();
	}

	public override void ModifyDrawData(ref Texture2D texture, ref Color color, ref int shader, Player associatedPlayer, Rectangle? sourceRectangle)
	{
		if ((secondDyeItem?.dye > 0) == true)
		{
			var target = DyeRenderTarget.GetAndRequestTargetInstance(new(associatedPlayer, texture, firstDyeItem.dye, sourceRectangle));
			if (target.IsReady)
			{
				texture = target.GetTarget();
			}
			shader = secondDyeItem.dye;

			if (secondDyeItem.hairDye > 0)
			{
				color = PlayerDrawHelpers.GetRawHairDyeColor(secondDyeItem.hairDye, associatedPlayer, color);
			}
		}
		else if ((firstDyeItem?.dye > 0) == true)
		{
			shader = firstDyeItem.dye;

			if (firstDyeItem.hairDye > 0)
			{
				color = PlayerDrawHelpers.GetRawHairDyeColor(firstDyeItem.hairDye, associatedPlayer, color);
			}
		}
		else
		{
			shader = 0;
		}
	}
}