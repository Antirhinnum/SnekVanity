using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SnekVanity.Common.ShopSelling;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SnekVanity.Content.CombinedDye;

public sealed class CombinedDyeItem : ModItem, IAmSoldByVanillaNPC
{
	private static class Hooks
	{
		public static void ReplacePlayerTextures(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
		{
			for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
			{
				DrawData data = drawinfo.DrawDataCache[i];
				// There's no way to get the Item this came from, so just clear it.
				if (data.shader == CombinedDyeShaderIndex)
				{
					data.shader = 0;
					drawinfo.DrawDataCache[i] = data;
					continue;
				}

				if (data.shader == 0 || !TryUnpackDyeValues(data.shader, out int first, out int second))
				{
					continue;
				}

				CombinedDyeRenderTarget target = CombinedDyeRenderTarget.GetAndRequestTargetInstance(drawinfo.drawPlayer, data.texture, first);
				if (target.IsReady)
				{
					data.texture = target.GetTarget();
				}

				data.shader = second;
				drawinfo.DrawDataCache[i] = data;
			}

			orig(ref drawinfo);
		}

		public static void ReplaceUnknownShader(On_Main.orig_PrepareDrawnEntityDrawing orig, Main self, Entity entity, int intendedShader, Matrix? overrideMatrix)
		{
			// There's no way to get the Item instance this dye value came from, so just clear it.
			if (intendedShader == CombinedDyeShaderIndex)
			{
				intendedShader = 0;
			}

			orig(self, entity, intendedShader, overrideMatrix);
		}

		// These three hooks are applied in the main Mod constructor, since they need to be done before any Main::EntitySpriteDraw() calls get inlined.
		public static void ReplaceEntityTexturesFloatScale(On_Main.orig_EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_float_SpriteEffects_float orig, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float worthless)
		{
			int originalShader = Main.CurrentDrawnEntityShader;
			TryReplacingTextureAndEntityShader(ref texture);
			orig(texture, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
			Main.CurrentDrawnEntityShader = originalShader;
		}

		public static void ReplaceEntityTexturesVector2Scale(On_Main.orig_EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_Vector2_SpriteEffects_float orig, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float worthless)
		{
			int originalShader = Main.CurrentDrawnEntityShader;
			TryReplacingTextureAndEntityShader(ref texture);
			orig(texture, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
			Main.CurrentDrawnEntityShader = originalShader;
		}

		public static void ReplaceEntityTexturesDrawData(On_Main.orig_EntitySpriteDraw_DrawData orig, DrawData data)
		{
			int originalShader = Main.CurrentDrawnEntityShader;
			TryReplacingTextureAndEntityShader(ref data.texture);
			orig(data);
			Main.CurrentDrawnEntityShader = originalShader;
		}

		private static void TryReplacingTextureAndEntityShader(ref Texture2D texture)
		{
			if (!TryUnpackDyeValues(Main.CurrentDrawnEntityShader, out int first, out int second))
			{
				return;
			}

			Player suppliedPlayer = null;
			if (Main.CurrentDrawnEntity is Player player)
			{
				suppliedPlayer = player;
			}
			else if (Main.CurrentDrawnEntity is Projectile projectile)
			{
				suppliedPlayer = Main.player[projectile.owner];
			}

			CombinedDyeRenderTarget target = CombinedDyeRenderTarget.GetAndRequestTargetInstance(suppliedPlayer, texture, first);
			if (target.IsReady)
			{
				texture = target.GetTarget();
			}
			Main.CurrentDrawnEntityShader = second;
		}
	}

	private static TooltipLine _noDyesLineCache;
	private static Asset<Texture2D> _dyeBottleAsset;
	private static Asset<Texture2D> _dyeFluidAsset;
	internal static int CombinedDyeShaderIndex { get; private set; }

	private Item _firstDyeItem, _secondDyeItem;

	int IAmSoldByVanillaNPC.NPC { get; } = NPCID.WitchDoctor;

	[field: CloneByReference]
	Condition IAmSoldByVanillaNPC.Available { get; } = Condition.Hardmode;

	public override void Load()
	{
		On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += Hooks.ReplacePlayerTextures;
		On_Main.PrepareDrawnEntityDrawing += Hooks.ReplaceUnknownShader;

		_dyeBottleAsset = ModContent.Request<Texture2D>(Texture + "_Bottle");
		_dyeFluidAsset = ModContent.Request<Texture2D>(Texture + "_Fluid");
	}

	// Called from the main Mod constructor.
	// If these hooks aren't done ASAP, then some Main::EntitySpriteDraw() calls get inlined -- notably, for projectile drawing.
	// This breaks the DIY Dye when used with Pet Shampoo or other projectile-dyeing items.
	internal static void DoSuperEarlyHooks()
	{
		On_Main.EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_float_SpriteEffects_float += Hooks.ReplaceEntityTexturesFloatScale;
		On_Main.EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_Vector2_SpriteEffects_float += Hooks.ReplaceEntityTexturesVector2Scale;
		On_Main.EntitySpriteDraw_DrawData += Hooks.ReplaceEntityTexturesDrawData;
	}

	public override void Unload()
	{
		_noDyesLineCache = null;
		_dyeBottleAsset = null;
		_dyeFluidAsset = null;
	}

	public override void SetStaticDefaults()
	{
		if (!Main.dedServ)
		{
			GameShaders.Armor.BindShader(Type, new ArmorShaderData(Main.Assets.Request<Effect>("PixelShader"), "Default"));
			CombinedDyeShaderIndex = GameShaders.Armor.GetShaderIdFromItemId(Type);
		}
	}

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.RedDye);
		Item.value = Item.buyPrice(gold: 10);

		// Just avoid the headache of dealing with stacks.
		// Avoid the issue of right-click both picking up one item and also being how you interact with this item.
		// Prevent duping dyes by stacking several of these and inserting one dye, then removing one-by-one.
		// Also prevents stacking items with different dyes inside, but that's achievable in other ways.
		Item.maxStack = 1;

		_firstDyeItem = new(0);
		_secondDyeItem = new(0);
		Item.dye = GetDyeValue();
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void RightClick(Player player)
	{
		if (Main.mouseItem == null || Main.mouseItem.IsAir)
		{
			if (_secondDyeItem != null && !_secondDyeItem.IsAir)
			{
				Main.mouseItem = ItemLoader.TransferWithLimit(_secondDyeItem, _secondDyeItem.maxStack);
			}
			else if (_firstDyeItem != null && !_firstDyeItem.IsAir)
			{
				Main.mouseItem = ItemLoader.TransferWithLimit(_firstDyeItem, _firstDyeItem.maxStack);
			}
		}
		else if (Main.mouseItem?.dye > 0 && Main.mouseItem.ModItem is not CombinedDyeItem) // No recursion, the system can't handle it.
		{
			if (_firstDyeItem == null || _firstDyeItem.IsAir)
			{
				_firstDyeItem = ItemLoader.TransferWithLimit(Main.mouseItem, 1);
			}
			else if (_secondDyeItem == null || _secondDyeItem.IsAir)
			{
				_secondDyeItem = ItemLoader.TransferWithLimit(Main.mouseItem, 1);
			}
		}

		Item.dye = GetDyeValue();
	}

	// Needed so right-clicking doesn't destroy the item.
	public override bool ConsumeItem(Player player)
	{
		return false;
	}

	public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		if (Item.dye == CombinedDyeShaderIndex)
		{
			return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
		}

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Main.UIScaleMatrix);

		// If this returns true: rendered and active are the correct dyes.
		// Otherwise: Don't try and pre-render a texture, just draw the one available dye.
		if (!TryUnpackDyeValues(Item.dye, out int rendered, out int active))
		{
			rendered = 0;
			active = Item.dye;
		}

		Texture2D fluidTexture = _dyeFluidAsset.Value;
		if (rendered > 0)
		{
			CombinedDyeRenderTarget target = CombinedDyeRenderTarget.GetAndRequestTargetInstance(Main.LocalPlayer, fluidTexture, rendered);
			if (target.IsReady)
			{
				fluidTexture = target.GetTarget();
			}
		}

		DrawData data = new(fluidTexture, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None)
		{
			shader = active
		};
		PlayerDrawHelper.SetShaderForData(Main.LocalPlayer, 0, ref data);

		// Special handling for hair dyes, since most of them just change the color parameter rather than using a shader.
		PlayerDrawHelper.UnpackShader(active, out int maybeHairShader, out PlayerDrawHelper.ShaderConfiguration shaderType);
		if (shaderType == PlayerDrawHelper.ShaderConfiguration.HairShader)
		{
			data.color = GameShaders.Hair.GetColor(maybeHairShader, Main.LocalPlayer, Color.White);
		}
		data.Draw(spriteBatch);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

		spriteBatch.Draw(_dyeBottleAsset.Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		if (Item.dye == CombinedDyeShaderIndex)
		{
			return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
		}

		Rectangle frame = _dyeBottleAsset.Frame();
		Vector2 origin = frame.Size() / 2f;
		Vector2 offset = new(Item.width / 2 - origin.X, Item.height - frame.Height);
		Vector2 drawPosition = Item.position - Main.screenPosition + origin + offset;

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

		// If this returns true: rendered and active are the correct dyes.
		// Otherwise: Don't try and pre-render a texture, just draw the one available dye.
		if (!TryUnpackDyeValues(Item.dye, out int rendered, out int active))
		{
			rendered = 0;
			active = Item.dye;
		}

		Texture2D fluidTexture = _dyeFluidAsset.Value;
		if (rendered > 0)
		{
			CombinedDyeRenderTarget target = CombinedDyeRenderTarget.GetAndRequestTargetInstance(Main.LocalPlayer, fluidTexture, rendered);
			if (target.IsReady)
			{
				fluidTexture = target.GetTarget();
			}
		}

		DrawData data = new(fluidTexture, drawPosition, frame, alphaColor, rotation, origin, scale, SpriteEffects.None)
		{
			shader = active
		};
		PlayerDrawHelper.SetShaderForData(Main.LocalPlayer, 0, ref data);

		// Special handling for hair dyes, since most of them just change the color parameter rather than using a shader.
		PlayerDrawHelper.UnpackShader(active, out int maybeHairShader, out PlayerDrawHelper.ShaderConfiguration shaderType);
		if (shaderType == PlayerDrawHelper.ShaderConfiguration.HairShader)
		{
			data.color = GameShaders.Hair.GetColor(maybeHairShader, Main.LocalPlayer, Color.White);
		}
		data.Draw(spriteBatch);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

		spriteBatch.Draw(_dyeBottleAsset.Value, drawPosition, frame, alphaColor, rotation, origin, scale, SpriteEffects.None, 0f);
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
		bool firstItemPresent = _firstDyeItem != null && !_firstDyeItem.IsAir;
		bool secondItemPresent = _secondDyeItem != null && !_secondDyeItem.IsAir;
		if (firstItemPresent && secondItemPresent)
		{
			toInsert = new(Mod, $"{Mod.Name}: {Name}DyeInfo", this.GetLocalization("MixingTwo").Format(_firstDyeItem.type, _secondDyeItem.type));
		}
		else if (firstItemPresent)
		{
			toInsert = new(Mod, $"{Mod.Name}: {Name}DyeInfo", this.GetLocalization("MixingOne").Format(_firstDyeItem.type));
		}
		else if (secondItemPresent)
		{
			toInsert = new(Mod, $"{Mod.Name}: {Name}DyeInfo", this.GetLocalization("MixingOne").Format(_secondDyeItem.type));
		}
		else
		{
			_noDyesLineCache ??= new TooltipLine(Mod, $"{Mod.Name}: {Name}DyeInfo", this.GetLocalizedValue("MixingNone"));
			toInsert = _noDyesLineCache;
		}

		// After last normal tooltip
		tooltips.Insert(lastTooltipIndex + 1, toInsert);
	}

	private int GetDyeValue()
	{
		// Using only Item::dye, we need to transfer both _firstDyeItem and _secondDyeItem's Item::dye values.
		// Dyes aren't considered if they're < -1, so we can't use negative values here.
		// Item::dye values are limited between [0, 4000) by PlayerDrawHelper::(Un)PackShader(), so we can get away with just bitwise operations.

		// Default to no dye.
		int dyeValue = CombinedDyeShaderIndex;

		// Pack if both are present.
		if (_firstDyeItem?.dye > 0 && _secondDyeItem?.dye > 0)
		{
			dyeValue = PackDyeValues((ushort)_firstDyeItem.dye, (ushort)_secondDyeItem.dye);
		}

		// Use individual dyes if one dye is missing.
		else if (_firstDyeItem?.dye > 0)
		{
			dyeValue = _firstDyeItem.dye;
		}
		else if (_secondDyeItem?.dye > 0)
		{
			dyeValue = _secondDyeItem.dye;
		}

		return dyeValue;
	}

	private static int PackDyeValues(ushort first, ushort second)
	{
		return (first << 16) | second;
	}

	private static bool TryUnpackDyeValues(int packed, out int first, out int second)
	{
		first = packed >> 16; // Top two bytes
		second = packed & 0xFFFF; // Bottom two bytes
		return first > 0 && second > 0;
	}

	public override ModItem Clone(Item newEntity)
	{
		CombinedDyeItem newItem = newEntity.ModItem as CombinedDyeItem;
		newItem._firstDyeItem = _firstDyeItem?.Clone();
		newItem._secondDyeItem = _secondDyeItem?.Clone();
		return base.Clone(newEntity);
	}

	public override void SaveData(TagCompound tag)
	{
		if (_firstDyeItem != null && !_firstDyeItem.IsAir)
		{
			tag.Add(nameof(_firstDyeItem), ItemIO.Save(_firstDyeItem));
		}

		if (_secondDyeItem != null && !_secondDyeItem.IsAir)
		{
			tag.Add(nameof(_secondDyeItem), ItemIO.Save(_secondDyeItem));
		}
	}

	public override void LoadData(TagCompound tag)
	{
		if (tag.TryGet(nameof(_firstDyeItem), out TagCompound firstItemTag))
		{
			_firstDyeItem = ItemIO.Load(firstItemTag);
		}
		if (tag.TryGet(nameof(_secondDyeItem), out TagCompound secondItemTag))
		{
			_secondDyeItem = ItemIO.Load(secondItemTag);
		}

		Item.dye = GetDyeValue();
	}

	public override void NetSend(BinaryWriter writer)
	{
		ItemIO.Send(_firstDyeItem ??= new(0), writer);
		ItemIO.Send(_secondDyeItem ??= new(0), writer);
	}

	public override void NetReceive(BinaryReader reader)
	{
		_firstDyeItem = ItemIO.Receive(reader);
		_secondDyeItem = ItemIO.Receive(reader);

		Item.dye = GetDyeValue();
	}
}