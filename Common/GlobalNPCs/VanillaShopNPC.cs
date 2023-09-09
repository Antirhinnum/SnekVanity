using SnekVanity.Core;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Common.GlobalNPCs;

public sealed class VanillaShopNPC : GlobalNPC
{
	private static readonly Dictionary<int, List<(IAmSoldByVanillaNPC, int)>> _soldItemsById = new();

	public override void Load()
	{
		_soldItemsById.Clear();
	}

	public override void Unload()
	{
		_soldItemsById.Clear();
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
			if (!_soldItemsById.ContainsKey(sold.NPC))
			{
				_soldItemsById[sold.NPC] = new();
			}
			_soldItemsById[sold.NPC].Add((sold, item.Type));
		}
	}

	public override void ModifyShop(NPCShop shop)
	{
		if (!_soldItemsById.TryGetValue(shop.NpcType, out List<(IAmSoldByVanillaNPC, int)> soldItems))
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