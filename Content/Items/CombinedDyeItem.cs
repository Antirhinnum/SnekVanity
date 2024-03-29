﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SnekVanity.Core;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SnekVanity.Content.Items;

public sealed class CombinedDyeItem : ModItem, IAmSoldByVanillaNPC
{
	private static class Hooks
	{
		public static void ReplaceTextures(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
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

		public static void ReplaceEntityTextures(On_Main.orig_EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_Vector2_SpriteEffects_float orig, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float worthless)
		{
			int originalShader = Main.CurrentDrawnEntityShader;
			if (TryUnpackDyeValues(Main.CurrentDrawnEntityShader, out int first, out int second))
			{
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

			orig(texture, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
			Main.CurrentDrawnEntityShader = originalShader;
		}

		public static void ReplaceEntityTexturesDrawData(On_Main.orig_EntitySpriteDraw_DrawData orig, DrawData data)
		{
			int originalShader = Main.CurrentDrawnEntityShader;
			if (TryUnpackDyeValues(Main.CurrentDrawnEntityShader, out int first, out int second))
			{
				Player suppliedPlayer = null;
				if (Main.CurrentDrawnEntity is Player player)
				{
					suppliedPlayer = player;
				}
				else if (Main.CurrentDrawnEntity is Projectile projectile)
				{
					suppliedPlayer = Main.player[projectile.owner];
				}

				CombinedDyeRenderTarget target = CombinedDyeRenderTarget.GetAndRequestTargetInstance(suppliedPlayer, data.texture, first);
				if (target.IsReady)
				{
					data.texture = target.GetTarget();
				}
				Main.CurrentDrawnEntityShader = second;
			}

			orig(data);
			Main.CurrentDrawnEntityShader = originalShader;
		}
	}

	// Draws textures under the influence of the first dye.
	public sealed class CombinedDyeRenderTarget : ARenderTargetContentByRequest, ILoadable
	{
		// We need one target per texture/shader combo, since we replace the textures being used with rendered versions.
		private static List<CombinedDyeRenderTarget> _targets;

		private Player _player;
		private Texture2D _texture;
		private int _shaderIndex;
		private bool _requestedSinceLastDraw;

		public void Load(Mod mod)
		{
			_targets = new();
			for (int i = 0; i < 4; i++)
			{
				AddTarget();
			}
		}

		public void Unload()
		{
			_targets?.Clear();
			_targets = null;
			Main.ContentThatNeedsRenderTargets.RemoveAll(t => t is CombinedDyeRenderTarget);
		}

		private static CombinedDyeRenderTarget AddTarget()
		{
			CombinedDyeRenderTarget newTarget = new();
			_targets.Add(newTarget);
			Main.ContentThatNeedsRenderTargets.Add(newTarget);
			return newTarget;
		}

		public static CombinedDyeRenderTarget GetAndRequestTargetInstance(Player player, Texture2D texture, int shader)
		{
			// If there's a target with the same data, use that one.
			// If not, use the first available target that hasn't been requested since it was last drawn.
			// If every available target has been requested since last draw, make a new one.
			CombinedDyeRenderTarget requestedTarget = _targets.FirstOrDefault(t => t._texture == texture && t._shaderIndex == shader);
			if (requestedTarget == null)
			{
				requestedTarget = _targets.FirstOrDefault(t => !t._requestedSinceLastDraw);
				requestedTarget ??= AddTarget();
			}

			requestedTarget.UseData(player, texture, shader);
			requestedTarget.Request();
			requestedTarget._requestedSinceLastDraw = true;
			return requestedTarget;
		}

		public void UseData(Player player, Texture2D texture, int shader)
		{
			_player = player;
			_texture = texture;
			_shaderIndex = shader;
		}

		protected override void HandleUseReqest(GraphicsDevice device, SpriteBatch spriteBatch)
		{
			_requestedSinceLastDraw = false;
			if (_player == null || _texture == null || _shaderIndex <= 0)
			{
				return;
			}

			PrepareARenderTarget_AndListenToEvents(ref _target, device, _texture.Width, _texture.Height, RenderTargetUsage.DiscardContents);
			device.SetRenderTarget(_target);
			device.Clear(Color.Transparent);
			DrawData value = new(_texture, Vector2.Zero, Color.White) { shader = _shaderIndex };
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			PlayerDrawHelper.SetShaderForData(_player, _player.cHead, ref value);
			value.Draw(spriteBatch);
			spriteBatch.End();
			device.SetRenderTarget(null);
			_wasPrepared = true;
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
		On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += Hooks.ReplaceTextures;
		On_Main.PrepareDrawnEntityDrawing += Hooks.ReplaceUnknownShader;
		On_Main.EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_Vector2_SpriteEffects_float += Hooks.ReplaceEntityTextures;
		On_Main.EntitySpriteDraw_DrawData += Hooks.ReplaceEntityTexturesDrawData;

		if (!Main.dedServ)
		{
			_dyeBottleAsset = ModContent.Request<Texture2D>(Texture + "_Bottle");
			_dyeFluidAsset = ModContent.Request<Texture2D>(Texture + "_Fluid");
		}
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
			GameShaders.Armor.BindShader(Type, new ArmorShaderData(Main.PixelShaderRef, "Default"));
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

		// If this return true: rendered and active are the correct dyes.
		// Otherwise: Don't try and pre-render a texture, just draw the one available dye .
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
		PlayerDrawHelper.SetShaderForData(null, 0, ref data);
		data.Draw(spriteBatch);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Main.UIScaleMatrix);

		spriteBatch.Draw(_dyeBottleAsset.Value, position, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
		return false;
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
	}

	public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
	{
		if (_firstDyeItem?.dye <= 0 && _secondDyeItem?.dye <= 0)
		{
			return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
		}

		Rectangle frame = _dyeBottleAsset.Frame();
		Vector2 origin = frame.Size() / 2f;
		Vector2 offset = new((Item.width / 2) - origin.X, Item.height - frame.Height);
		Vector2 drawPosition = Item.position - Main.screenPosition + origin + offset;

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

		// If this return true: rendered and active are the correct dyes.
		// Otherwise: Don't try and pre-render a texture, just draw the one available dye .
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
		PlayerDrawHelper.SetShaderForData(null, 0, ref data);
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

	public int GetDyeValue()
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

	public static int PackDyeValues(ushort first, ushort second)
	{
		return (first << 16) | second;
	}

	public static bool TryUnpackDyeValues(int packed, out int first, out int second)
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