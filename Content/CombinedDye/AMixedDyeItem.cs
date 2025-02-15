using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SnekVanity.Common.CustomDyes;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.UI.Chat;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SnekVanity.Content.CombinedDye;

/// <summary>
/// A dye that mixes two other dyes in some fashion.
/// </summary>
public abstract class AMixedDyeItem : ACustomDyeItem
{
	private static Dictionary<int, (Asset<Texture2D> Bottle, Asset<Texture2D> Fluid)> _assetsByType;
	private static TooltipLine _noDyesLineCache;
	private static bool _hooked;

	protected Item firstDyeItem, secondDyeItem;

	public override bool HasAnyEffects => firstDyeItem?.dye > 0;
	protected virtual string EmptyBottleTexture => Texture + "_Bottle";
	protected virtual string BottleFluidTexture => Texture + "_Fluid";
	protected virtual bool CanAcceptUselessDye => true;

	public override void Load()
	{
		if (!_hooked)
		{
			_hooked = true;
			On_Item.GetShimmered += PreserveDyesAcrossShimmer;
		}
	}

	// Preserve contained dyes across shimmering, since the existing mixed dyes can all be shimmered into each other.
	private static void PreserveDyesAcrossShimmer(On_Item.orig_GetShimmered orig, Item self)
	{
		Item dye1 = null;
		Item dye2 = null;
		if (self.ModItem is AMixedDyeItem mixedBefore)
		{
			(dye1, dye2) = (mixedBefore.firstDyeItem, mixedBefore.secondDyeItem);
		}

		orig(self);

		if (dye1 != null && self.ModItem is AMixedDyeItem mixedAfter)
		{
			(mixedAfter.firstDyeItem, mixedAfter.secondDyeItem) = (dye1, dye2);
			mixedAfter.CacheSelf();
		}
	}

	public override void Unload()
	{
		base.Unload();

		_noDyesLineCache = null;
		_assetsByType?.Clear();
		_assetsByType = null;
	}

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();

		_assetsByType ??= [];
		_assetsByType[Type] = (ModContent.Request<Texture2D>(EmptyBottleTexture), ModContent.Request<Texture2D>(BottleFluidTexture));
	}

	public override void SetDefaults()
	{
		firstDyeItem = new(0);
		secondDyeItem = new(0);

		base.SetDefaults();
		CacheSelf();

		Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 10));
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void RightClick(Player player)
	{
		if (Main.mouseItem == null || Main.mouseItem.IsAir)
		{
			if (secondDyeItem != null && !secondDyeItem.IsAir)
			{
				Main.mouseItem = ItemLoader.TransferWithLimit(secondDyeItem, secondDyeItem.maxStack);
			}
			else if (firstDyeItem != null && !firstDyeItem.IsAir)
			{
				Main.mouseItem = ItemLoader.TransferWithLimit(firstDyeItem, firstDyeItem.maxStack);
			}
		}
		else if ((CanAcceptUselessDye && Main.mouseItem?.ModItem is UselessDyeItem) // Accept a useless dye if allowed
			|| (Main.mouseItem?.dye > 0))
		{
			if (firstDyeItem == null || firstDyeItem.IsAir)
			{
				firstDyeItem = ItemLoader.TransferWithLimit(Main.mouseItem, 1);
			}
			else if (secondDyeItem == null || secondDyeItem.IsAir)
			{
				secondDyeItem = ItemLoader.TransferWithLimit(Main.mouseItem, 1);
			}
		}

		CacheSelf();
	}

	// Needed so right-clicking doesn't destroy the item.
	public override bool ConsumeItem(Player player)
	{
		return false;
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (!HasAnyEffects)
		{
			return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
		}

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);

		Texture2D fluidTexture = _assetsByType[Type].Fluid.Value;
		Color fluidColor = drawColor;
		int shader = 0;
		ModifyDrawData(ref fluidTexture, ref fluidColor, ref shader, Main.LocalPlayer, frame);

		DrawData data = new(fluidTexture, position, frame, fluidColor, 0f, origin, scale, SpriteEffects.None)
		{
			shader = shader
		};
		PlayerDrawHelper.SetShaderForData(Main.LocalPlayer, 0, ref data);

		// Special handling for hair dyes, since most of them just change the color parameter rather than using a shader.
		PlayerDrawHelper.UnpackShader(shader, out int maybeHairShader, out PlayerDrawHelper.ShaderConfiguration shaderType);
		if (shaderType == PlayerDrawHelper.ShaderConfiguration.HairShader)
		{
			data.color = GameShaders.Hair.GetColor(maybeHairShader, Main.LocalPlayer, Color.White);
		}
		data.Draw(spriteBatch);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

		spriteBatch.Draw(_assetsByType[Type].Bottle.Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		if (!HasAnyEffects)
		{
			return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
		}

		Rectangle frame = _assetsByType[Type].Bottle.Frame();
		Vector2 origin = frame.Size() / 2f;
		Vector2 offset = new(Item.width / 2 - origin.X, Item.height - frame.Height);
		Vector2 drawPosition = Item.position - Main.screenPosition + origin + offset;

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

		Texture2D fluidTexture = _assetsByType[Type].Fluid.Value;
		Color fluidColor = alphaColor;
		int shader = 0;
		ModifyDrawData(ref fluidTexture, ref fluidColor, ref shader, Main.LocalPlayer, frame);

		DrawData data = new(fluidTexture, drawPosition, frame, fluidColor, rotation, origin, scale, SpriteEffects.None)
		{
			shader = shader
		};
		PlayerDrawHelper.SetShaderForData(Main.LocalPlayer, 0, ref data);

		// Special handling for hair dyes, since most of them just change the color parameter rather than using a shader.
		PlayerDrawHelper.UnpackShader(shader, out int maybeHairShader, out PlayerDrawHelper.ShaderConfiguration shaderType);
		if (shaderType == PlayerDrawHelper.ShaderConfiguration.HairShader)
		{
			data.color = PlayerDrawHelpers.GetRawHairDyeColor(maybeHairShader, Main.LocalPlayer);
		}
		data.Draw(spriteBatch);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

		spriteBatch.Draw(_assetsByType[Type].Bottle.Value, drawPosition, frame, alphaColor, rotation, origin, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		int lastTooltipIndex = tooltips.FindLastIndex(t => t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"));
		if (lastTooltipIndex == -1)
		{
			return;
		}

		TooltipLine toInsert;
		bool firstItemPresent = firstDyeItem != null && !firstDyeItem.IsAir;
		bool secondItemPresent = secondDyeItem != null && !secondDyeItem.IsAir;
		if (firstItemPresent && secondItemPresent)
		{
			toInsert = new(Mod, $"{Mod.Name}: {Name}DyeInfo", Language.GetText("Mods.SnekVanity.Items.MixedDyeCommon.MixingTwo").Format(ItemTagHandler.GenerateTag(firstDyeItem), ItemTagHandler.GenerateTag(secondDyeItem)));
		}
		else if (firstItemPresent)
		{
			toInsert = new(Mod, $"{Mod.Name}: {Name}DyeInfo", Language.GetText("Mods.SnekVanity.Items.MixedDyeCommon.MixingOne").Format(ItemTagHandler.GenerateTag(firstDyeItem)));
		}
		else if (secondItemPresent)
		{
			toInsert = new(Mod, $"{Mod.Name}: {Name}DyeInfo", Language.GetText("Mods.SnekVanity.Items.MixedDyeCommon.MixingOne").Format(ItemTagHandler.GenerateTag(secondDyeItem)));
		}
		else
		{
			_noDyesLineCache ??= new TooltipLine(Mod, $"{Mod.Name}: {Name}DyeInfo", Language.GetTextValue("Mods.SnekVanity.Items.MixedDyeCommon.MixingNone"));
			toInsert = _noDyesLineCache;
		}

		// After last normal tooltip
		tooltips.Insert(lastTooltipIndex + 1, toInsert);
	}

	public override ModItem Clone(Item newEntity)
	{
		AMixedDyeItem newItem = base.Clone(newEntity) as AMixedDyeItem;
		newItem.firstDyeItem = firstDyeItem?.Clone();
		newItem.secondDyeItem = secondDyeItem?.Clone();
		CacheSelf();
		return newItem;
	}

	public override void SaveData(TagCompound tag)
	{
		if (firstDyeItem != null && !firstDyeItem.IsAir)
		{
			tag.Add(nameof(firstDyeItem), ItemIO.Save(firstDyeItem));
		}

		if (secondDyeItem != null && !secondDyeItem.IsAir)
		{
			tag.Add(nameof(secondDyeItem), ItemIO.Save(secondDyeItem));
		}
	}

	public override void LoadData(TagCompound tag)
	{
		if (tag.TryGet(nameof(firstDyeItem), out TagCompound firstItemTag))
		{
			firstDyeItem = ItemIO.Load(firstItemTag);
		}
		if (tag.TryGet(nameof(secondDyeItem), out TagCompound secondItemTag))
		{
			secondDyeItem = ItemIO.Load(secondItemTag);
		}

		CacheSelf();
	}

	public override void NetSend(BinaryWriter writer)
	{
		ItemIO.Send(firstDyeItem ??= new(0), writer);
		ItemIO.Send(secondDyeItem ??= new(0), writer);
	}

	public override void NetReceive(BinaryReader reader)
	{
		firstDyeItem = ItemIO.Receive(reader);
		secondDyeItem = ItemIO.Receive(reader);

		CacheSelf();
	}

	public override void CacheSelf()
	{
		base.CacheSelf();

		if (firstDyeItem?.ModItem is ACustomDyeItem firstCustomDye)
		{
			firstCustomDye.CacheSelf();
		}
		if (secondDyeItem?.ModItem is ACustomDyeItem secondCustomDye)
		{
			secondCustomDye.CacheSelf();
		}
	}

	public override void UncacheSelf()
	{
		base.UncacheSelf();

		if (firstDyeItem?.ModItem is ACustomDyeItem firstCustomDye)
		{
			firstCustomDye.UncacheSelf();
		}
		if (secondDyeItem?.ModItem is ACustomDyeItem secondCustomDye)
		{
			secondCustomDye.UncacheSelf();
		}
	}
}