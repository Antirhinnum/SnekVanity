using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Common.ShopSelling;

/// <summary>
/// Implements the functionality of <see cref="IAmSoldByVanillaNPC"/>.
/// </summary>
public sealed class VanillaShopNPC : GlobalNPC
{
	private record struct SaleInfo(IAmSoldByVanillaNPC Interface, int ItemType);

	private static Dictionary<int, List<SaleInfo>> _soldItemsById;

	public override void Load()
	{
		_soldItemsById = [];
	}

	public override void Unload()
	{
		_soldItemsById?.Clear();
		_soldItemsById = null;
	}

	public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
	{
		return (entity.townNPC || entity.isLikeATownNPC) && entity.type < NPCID.Count;
	}

	public override void SetStaticDefaults()
	{
		foreach (ModItem item in Mod.GetContent<ModItem>().Where(m => m is IAmSoldByVanillaNPC))
		{
			IAmSoldByVanillaNPC sold = item as IAmSoldByVanillaNPC;
			if (!_soldItemsById.TryGetValue(sold.NPC, out List<SaleInfo> value))
			{
				value = [];
				_soldItemsById[sold.NPC] = value;
			}

			value.Add(new(sold, item.Type));
		}
	}

	public override void ModifyShop(NPCShop shop)
	{
		if (!_soldItemsById.TryGetValue(shop.NpcType, out List<SaleInfo> soldItems))
		{
			return;
		}

		foreach ((IAmSoldByVanillaNPC condition, int type) in soldItems)
		{
			if (condition.Available != null)
			{
				shop.Add(type, condition.Available);
			}
			else
			{
				shop.Add(type);
			}
		}
	}
}