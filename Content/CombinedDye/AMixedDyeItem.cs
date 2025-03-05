using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SnekVanity.Common.CustomDyes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.UI.Chat;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SnekVanity.Content.CombinedDye;

/// <summary>
/// A dye that mixes up to four other dyes in some fashion.
/// </summary>
public abstract class AMixedDyeItem : ACustomDyeItem
{
	private static Dictionary<int, (Asset<Texture2D> Bottle, Asset<Texture2D> Fluid)> _assetsByType;
	private static TooltipLine _noDyesLineCache, _showDyeNamesLineCache;
	private static bool _hooked;

	protected const int MAX_DYES_LIMIT = 4;
	protected Item[] dyeItems = new Item[MAX_DYES_LIMIT];

	protected override bool CloneNewInstances => true;
	public override bool HasAnyEffects => dyeItems?.Any(ItemIsValidDye) ?? false;
	protected virtual string EmptyBottleTexture => Texture + "_Bottle";
	protected virtual string BottleFluidTexture => Texture + "_Fluid";
	protected virtual bool CanAcceptUselessDye => true;
	protected virtual int MaxDyesAllowed => MAX_DYES_LIMIT;
	protected int MaxDyes => Math.Min(MAX_DYES_LIMIT, MaxDyesAllowed);

	public override void Load()
	{
		if (!_hooked)
		{
			_hooked = true;
			On_Item.GetShimmered += PreserveDyesAcrossShimmer;
		}
	}

	protected static bool ItemIsValidDye(Item item) => ItemIsValid(item) && item.dye > 0;

	protected static bool ItemIsValid(Item item) => item != null && !item.IsAir;

	// Preserve contained dyes across shimmering, since the existing mixed dyes can all be shimmered into each other.
	private static void PreserveDyesAcrossShimmer(On_Item.orig_GetShimmered orig, Item self)
	{
		Item[] dyes = null;
		if (self.ModItem is AMixedDyeItem mixedBefore)
		{
			dyes = mixedBefore.dyeItems;
		}

		orig(self);

		if (dyes != null && self.ModItem is AMixedDyeItem mixedAfter)
		{
			mixedAfter.dyeItems = dyes;
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
		dyeItems = new Item[MAX_DYES_LIMIT];
		for (int i = 0; i < MAX_DYES_LIMIT; i++)
		{
			dyeItems[i] = new Item(ItemID.None);
			dyeItems[i].TurnToAir();
		}

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
		if (dyeItems == null)
		{
			dyeItems = new Item[MAX_DYES_LIMIT];
			for (int i = 0; i < MAX_DYES_LIMIT; i++)
			{
				dyeItems[i] = new Item(ItemID.None);
			}
		}

		if (!ItemIsValid(Main.mouseItem))
		{
			for (int i = MAX_DYES_LIMIT - 1; i >= 0; i--)
			{
				if (ItemIsValid(dyeItems[i]))
				{
					Main.mouseItem = ItemLoader.TransferWithLimit(dyeItems[i], dyeItems[i].maxStack);
					break;
				}
			}
		}
		else if (Main.mouseItem != null && CanAcceptItem(Main.mouseItem))
		{
			// Only allow insertion up to the limited amount
			for (int i = 0; i < MaxDyes; i++)
			{
				if (!ItemIsValid(dyeItems[i]))
				{
					dyeItems[i] = ItemLoader.TransferWithLimit(Main.mouseItem, 1);
					break;
				}
			}
		}

		CacheSelf();
	}

	protected virtual bool CanAcceptItem(Item dye)
	{
		if (dye.ModItem is UselessDyeItem && CanAcceptUselessDye)
		{
			return true;
		}

		return dye.dye > 0;
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
		int shader = Item.dye;
		CustomDyeHooks.ModifyDrawData(ref fluidTexture, ref fluidColor, ref shader, Main.LocalPlayer, frame);

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
		int shader = Item.dye;
		CustomDyeHooks.ModifyDrawData(ref fluidTexture, ref fluidColor, ref shader, Main.LocalPlayer, frame);

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

		IEnumerable<Item> itemsToList = dyeItems?.TakeWhile(ItemIsValid);
		if (!itemsToList.Any())
		{
			_noDyesLineCache ??= new TooltipLine(Mod, $"{Mod.Name}: {Name}DyeInfo", Language.GetTextValue("Mods.SnekVanity.Items.MixedDyeCommon.MixingNone"));
			tooltips.Insert(lastTooltipIndex + 1, _noDyesLineCache);
		}
		else
		{
			string text = Language.GetTextValue("Mods.SnekVanity.Items.MixedDyeCommon.Mixing");
			if (Main.keyState.PressingShift())
			{
				text += '\n' + string.Join('\n', itemsToList.Select(i => Language.GetText("Mods.SnekVanity.Items.MixedDyeCommon.MixingListItem").Format(ItemTagHandler.GenerateTag(i), Lang.GetItemName(i.type))));
			}
			else
			{
				text += ' ' + string.Join(", ", itemsToList.Select(ItemTagHandler.GenerateTag));
			}

			TooltipLine infoLine = new(Mod, $"{Mod.Name}: {Name}DyeInfo", text);
			tooltips.Insert(lastTooltipIndex + 1, infoLine);

			if (!Main.keyState.PressingShift())
			{
				_showDyeNamesLineCache ??= new TooltipLine(Mod, $"{Mod.Name}: {Name}Hint", Language.GetTextValue("Mods.SnekVanity.Items.MixedDyeCommon.MixingTooltipHint"));
				tooltips.Insert(lastTooltipIndex + 2, _showDyeNamesLineCache);
			}
		}
	}

	public override ModItem Clone(Item newEntity)
	{
		CacheSelf();
		AMixedDyeItem newItem = base.Clone(newEntity) as AMixedDyeItem;
		newItem.dyeItems = new Item[MAX_DYES_LIMIT];
		for (int i = 0; i < MAX_DYES_LIMIT; i++)
		{
			newItem.dyeItems[i] = dyeItems[i]?.Clone();
		}
		newItem.CacheSelf();
		return newItem;
	}

	public override void SaveData(TagCompound tag)
	{
		IEnumerable<Item> dyesToSave = dyeItems?.TakeWhile(ItemIsValid);
		if (dyesToSave.Any())
		{
			tag.Add(nameof(dyeItems), dyesToSave.Select(ItemIO.Save).ToList());
		}
	}

	public override void LoadData(TagCompound tag)
	{
		if (tag.TryGet(nameof(dyeItems), out List<TagCompound> dyeItemsTag))
		{
			int count = Math.Min(MAX_DYES_LIMIT, dyeItemsTag.Count);
			for (int i = 0; i < count; i++)
			{
				ItemIO.Load(dyeItems[i], dyeItemsTag[i]);
			}
		}

		// Legacy
		if (tag.TryGet("firstDyeItem", out TagCompound firstItemTag) || tag.TryGet("_firstDyeItem", out firstItemTag))
		{
			ItemIO.Load(dyeItems[0], firstItemTag);
		}
		if (tag.TryGet("secondDyeItem", out TagCompound secondItemTag) || tag.TryGet("_secondDyeItem", out secondItemTag))
		{
			ItemIO.Load(dyeItems[1], secondItemTag);
		}

		CacheSelf();
	}

	public override void NetSend(BinaryWriter writer)
	{
		int count = dyeItems?.TakeWhile(ItemIsValid).Count() ?? 0;
		writer.Write((byte)count);
		for (int i = 0; i < count; i++)
		{
			ItemIO.Send(dyeItems[i], writer);
		}
	}

	public override void NetReceive(BinaryReader reader)
	{
		int count = reader.ReadByte();
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				ItemIO.Receive(dyeItems[i], reader);
			}
			CacheSelf();
		}
	}

	public override void CacheSelf()
	{
		base.CacheSelf();

		if (dyeItems == null)
		{
			return;
		}

		for (int i = 0; i < MAX_DYES_LIMIT; i++)
		{
			if (dyeItems[i]?.ModItem is ACustomDyeItem customDye)
			{
				customDye.CacheSelf();
			}
		}
	}

	public override void UncacheSelf()
	{
		base.UncacheSelf();

		if (dyeItems == null)
		{
			return;
		}

		for (int i = 0; i < MAX_DYES_LIMIT; i++)
		{
			if (dyeItems[i]?.ModItem is ACustomDyeItem customDye)
			{
				customDye.UncacheSelf();
			}
		}
	}

	public override ushort GetUniqueDyeIndex()
	{
		HashCode hash = new();
		foreach (Item item in dyeItems.Where(ItemIsValid))
		{
			hash.Add(item.dye);
		}
		ushort hashFinal = (ushort)(hash.ToHashCode() & 0xFFFF);
		return (ushort)(hash.ToHashCode() & 0xFFFF);
	}
}