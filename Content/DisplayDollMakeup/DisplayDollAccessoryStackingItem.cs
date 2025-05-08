using SnekVanity.Common.EquipDuplicateAccessories;
using SnekVanity.Common.ShopSelling;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent.UI.Chat;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.IO;

namespace SnekVanity.Content.DisplayDollMakeup;

public sealed class DisplayDollAccessoryStackingItem : ModItem, IAmSoldByVanillaNPC
{
	private static MethodInfo _playerUpdateItemDye;
	private static TooltipLine _containsNothingTooltip;

	public Stack<Item> StoredItems { get; private set; }

	protected override bool CloneNewInstances => true;

	int IAmSoldByVanillaNPC.NPC { get; } = NPCID.Clothier;

	public override void Load()
	{
		_playerUpdateItemDye = typeof(Player).GetMethod("UpdateItemDye", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

		On_Player.UpdateVisibleAccessory += UpdateInnerAccessories;
		On_Player.UpdateItemDye += UpdateInnerAccessoriesDye;
	}

	public override void Unload()
	{
		_playerUpdateItemDye = null;
	}

	private static void UpdateInnerAccessories(On_Player.orig_UpdateVisibleAccessory orig, Player self, int itemSlot, Item item, bool modded)
	{
		orig(self, itemSlot, item, modded);

		if (item.ModItem is DisplayDollAccessoryStackingItem stacking)
		{
			foreach (Item innerItem in stacking.StoredItems)
			{
				if (!self.ItemIsVisuallyIncompatible(innerItem))
				{
					self.UpdateVisibleAccessory(itemSlot, innerItem, modded);
				}
			}
		}
	}

	private static void UpdateInnerAccessoriesDye(On_Player.orig_UpdateItemDye orig, Player self, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem)
	{
		orig(self, isNotInVanitySlot, isSetToHidden, armorItem, dyeItem);

		if (armorItem.ModItem is DisplayDollAccessoryStackingItem stacking)
		{
			foreach (Item innerItem in stacking.StoredItems)
			{
				// We *could* just invoke orig() here, but that might skip other mods' hooks into UpdateItemDye().
				// Unfortunately, the method is internal, so reflection it is.
				_playerUpdateItemDye.Invoke(self, [isNotInVanitySlot, isSetToHidden, innerItem, dyeItem]);
			}
		}
	}

	public override void SetStaticDefaults()
	{
		EquipDuplicateAccessoriesSystem.AccessoryEquippableMultipleTimes[Type] = true;
	}

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.SetShopValues(ItemRarityColor.Blue1, Item.buyPrice(gold: 1));
		Item.vanity = true;
		Item.hasVanityEffects = true;
		Item.maxStack = 1;
		StoredItems = new();
	}

	public override bool CanEquipAccessory(Player player, int slot, bool modded)
	{
		if (modded && player.TryGetModPlayer(out ModAccessorySlotPlayer accessoryPlayer))
		{
			// First half of the array is normal slots, second half is vanity slots
			return slot >= accessoryPlayer.SlotCount;
		}
		return player.isDisplayDollOrInanimate || (slot >= 10 && slot <= 17);
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void RightClick(Player player)
	{
		if ((Main.mouseItem == null || Main.mouseItem.IsAir) && StoredItems.TryPop(out Item topItem))
		{
			Main.mouseItem = ItemLoader.TransferWithLimit(topItem, topItem.stack);
		}
		else if (Main.mouseItem != null && !Main.mouseItem.IsAir && Main.mouseItem.accessory && Main.mouseItem.ModItem is not DisplayDollAccessoryStackingItem) // No recursion
		{
			Item itemToStore = ItemLoader.TransferWithLimit(Main.mouseItem, Main.mouseItem.stack);
			StoredItems.Push(itemToStore);
		}
	}

	// Needed so right-clicking doesn't destroy the item.
	public override bool ConsumeItem(Player player)
	{
		return false;
	}

	public override void ModifyTooltips(List<TooltipLine> tooltips)
	{
		int lastTooltipLineIndex = tooltips.FindLastIndex(t => t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"));
		if (lastTooltipLineIndex == -1)
		{
			return;
		}

		if (StoredItems.Count == 0)
		{
			_containsNothingTooltip ??= new TooltipLine(Mod, "TooltipContainsNothing", this.GetLocalizedValue("ContainsNothingTooltip"));
			tooltips.Insert(lastTooltipLineIndex + 1, _containsNothingTooltip);
		}
		else
		{
			string items = string.Join(", ", StoredItems.Reverse().Select(ItemTagHandler.GenerateTag));
			tooltips.Insert(lastTooltipLineIndex + 1, new TooltipLine(Mod, "TooltipContents", this.GetLocalizedValue("ContainsTooltip") + items));
		}
	}

	public override ModItem Clone(Item newEntity)
	{
		DisplayDollAccessoryStackingItem clone = base.Clone(newEntity) as DisplayDollAccessoryStackingItem;
		clone.StoredItems = new();
		foreach (Item storedItem in StoredItems?.Reverse()) // Reverse order so that the first item pushed in the cloned stack is also the first item pushed to this one.
		{
			clone.StoredItems.Push(storedItem.Clone());
		}
		return clone;
	}

	public override void SaveData(TagCompound tag)
	{
		if (StoredItems.Count != 0)
		{
			tag[nameof(StoredItems)] = StoredItems.Reverse().Select(ItemIO.Save).ToList();
		}
	}

	public override void LoadData(TagCompound tag)
	{
		if (tag.TryGet(nameof(StoredItems), out List<TagCompound> items))
		{
			StoredItems = new();
			foreach (Item item in items.Select(ItemIO.Load))
			{
				StoredItems.Push(item);
			}
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write7BitEncodedInt(StoredItems.Count);
		foreach (Item item in StoredItems.Reverse())
		{
			ItemIO.Send(item, writer, writeStack: true);
		}
	}

	public override void NetReceive(BinaryReader reader)
	{
		int count = reader.Read7BitEncodedInt();
		for (int i = 0; i < count; i++)
		{
			StoredItems.Push(ItemIO.Receive(reader, readStack: true));
		}
	}
}