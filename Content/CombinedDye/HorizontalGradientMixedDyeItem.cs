using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Common.ShopSelling;
using SnekVanity.Content.GradientDye;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.CombinedDye;

public sealed class HorizontalGradientMixedDyeItem : AMixedDyeItem, IAmSoldByVanillaNPC
{
	[field: CloneByReference]
	Condition IAmSoldByVanillaNPC.Available { get; } = Condition.Hardmode;

	int IAmSoldByVanillaNPC.NPC { get; } = NPCID.WitchDoctor;

	public override bool HasAnyEffects => (firstDyeItem?.dye > 0) || (secondDyeItem?.dye > 0);
	protected override string BottleFluidTexture => Texture.Replace(Name, nameof(OverlaidMixedDyeItem)) + "_Fluid";

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<OverlaidMixedDyeItem>();
	}

	public override void ModifyDrawData(ref Texture2D texture, ref Color color, ref int shader, Player associatedPlayer, Rectangle? sourceRectangle = null)
	{
		int firstDye = firstDyeItem?.dye ?? -1;
		int secondDye = secondDyeItem?.dye ?? -1;
		if (firstDye <= 0 && secondDye <= 0)
		{
			return;
		}

		var target = HorizontalGradientDyeRenderTarget.GetAndRequestTargetInstance(new(associatedPlayer, texture, firstDyeItem?.dye ?? 0, secondDyeItem?.dye ?? 0, sourceRectangle));
		if (target.IsReady)
		{
			texture = target.GetTarget();
		}
		shader = 0;
	}
}