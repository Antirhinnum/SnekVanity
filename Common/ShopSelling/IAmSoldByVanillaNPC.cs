using Terraria;
using Terraria.ModLoader;

namespace SnekVanity.Common.ShopSelling;

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will be automatically added to the given NPC's shop.
/// </summary>
public interface IAmSoldByVanillaNPC
{
	/// <summary>
	/// The ID of the NPC that sells this item.
	/// </summary>
	int NPC { get; }

	/// <summary>
	/// The condition under which this item is sold. If <see langword="null"/>, then the item is always sold.
	/// <br/> Defaults to <see langword="null"/>.
	/// </summary>
	Condition Available => null;
}