using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnekVanity.Content.DyePlayerTextures;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SnekVanity.Content.DisplayDollMakeup;

public sealed class DisplayDollMakeupItem : ModItem
{
	private static TooltipLine _noSavedStyleTooltipLine;
	private static TooltipLine _savedStyleTooltipLine;
	private static Player _tooltipDummyPlayer;

	internal SavedPlayerStyleValues? SavedStyleValues { get; private set; }

	protected override bool CloneNewInstances => true;

	public override void Unload()
	{
		_noSavedStyleTooltipLine = null;
		_savedStyleTooltipLine = null;
		_tooltipDummyPlayer = null;
	}

	public override void SetStaticDefaults()
	{
		_tooltipDummyPlayer = new();
		ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<AllSkinDyeItem>();
	}

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 1));

		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTime = Item.useAnimation = 60;
	}

	public override bool AltFunctionUse(Player player)
	{
		return true;
	}

	public override bool? UseItem(Player player)
	{
		if (player.altFunctionUse == ItemAlternativeFunctionID.ActivatedAndUsed && SavedStyleValues.HasValue)
		{
			SavedStyleValues.Value.Retrieve(player);
		}
		else
		{
			SavedStyleValues = new(player);
		}

		return true;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		int lastTooltipLineIndex = tooltips.FindLastIndex(t => t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"));
		if (lastTooltipLineIndex == -1)
		{
			return;
		}

		if (!SavedStyleValues.HasValue)
		{
			_noSavedStyleTooltipLine ??= new TooltipLine(Mod, "TooltipNoSavedStyle", this.GetLocalizedValue("NoSavedStyleTooltip"));
			tooltips.Insert(lastTooltipLineIndex + 1, _noSavedStyleTooltipLine);
		}
		else
		{
			_savedStyleTooltipLine = new TooltipLine(Mod, "TooltipSavedStyle", this.GetLocalizedValue("CurrentSavedStyleTooltip"));
			tooltips.Insert(lastTooltipLineIndex + 1, _savedStyleTooltipLine);
			tooltips.Insert(lastTooltipLineIndex + 2, new TooltipLine(Mod, "Spacer", " "));
		}
	}

	public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
	{
		if (line.Name == "TooltipSavedStyle")
		{
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

			Vector2 offset = line.Font.MeasureString(line.Text) + new Vector2(16f, 0f);
			SavedStyleValues.Value.Retrieve(_tooltipDummyPlayer);
			_tooltipDummyPlayer.position = new Vector2(line.X + offset.X, line.Y) + Main.screenPosition;
			_tooltipDummyPlayer.PlayerFrame();
			_tooltipDummyPlayer.socialIgnoreLight = true;
			Main.PlayerRenderer.DrawPlayer(Main.Camera, _tooltipDummyPlayer, _tooltipDummyPlayer.position, _tooltipDummyPlayer.fullRotation, _tooltipDummyPlayer.fullRotationOrigin);
			//yOffset = 20;

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.SamplerStateForCursor, null, null, null, Main.UIScaleMatrix);
		}
		return base.PreDrawTooltipLine(line, ref yOffset);
	}

	public override ModItem Clone(Item newEntity)
	{
		DisplayDollMakeupItem clone = base.Clone(newEntity) as DisplayDollMakeupItem;
		if (SavedStyleValues.HasValue)
		{
			clone.SavedStyleValues = new SavedPlayerStyleValues(SavedStyleValues.Value);
		}
		return clone;
	}

	public override void SaveData(TagCompound tag)
	{
		if (SavedStyleValues.HasValue)
		{
			tag.Add(nameof(SavedStyleValues), SavedStyleValues.Value.SaveData());
		}
	}

	public override void LoadData(TagCompound tag)
	{
		if (tag.TryGet(nameof(SavedStyleValues), out TagCompound styles))
		{
			SavedStyleValues = default(SavedPlayerStyleValues).LoadData(styles);
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(SavedStyleValues.HasValue);
		if (SavedStyleValues.HasValue)
		{
			SavedStyleValues.Value.Send(writer);
		}
	}

	public override void NetReceive(BinaryReader reader)
	{
		if (reader.ReadBoolean())
		{
			SavedStyleValues = default(SavedPlayerStyleValues).Receive(reader);
		}
	}
}