using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.ShopSelling;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.CombinedDye;

[LegacyName("VerticalGradientMixedDye")]
public sealed class VerticalGradientMixedDyeItem : AMixedDyeItem, IAmSoldByVanillaNPC
{
	[field: CloneByReference]
	Condition IAmSoldByVanillaNPC.Available { get; } = Condition.Hardmode;

	int IAmSoldByVanillaNPC.NPC { get; } = NPCID.WitchDoctor;

	protected override string BottleFluidTexture => Texture.Replace(Name, nameof(OverlaidMixedDyeItem)) + "_Fluid";

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<HorizontalGradientMixedDyeItem>();
	}

	public override void ModifyDrawData(ref Texture2D texture, ref Color color, ref int shader, Player associatedPlayer, Rectangle? sourceRectangle = null)
	{
		ACachedRenderTarget<VerticalGradientDyeRenderTarget, VerticalGradientDyeRenderTarget.Data> target =
			VerticalGradientDyeRenderTarget.GetAndRequestTargetInstance(new(
				associatedPlayer,
				texture,
				sourceRectangle,
				shaderIndices: dyeItems.Take(MaxDyes).Where(ItemIsValid).Select(i => i.dye).ToArray()));
		if (target.IsReady)
		{
			texture = target.GetTarget();
		}
		shader = 0;
	}
}