using Terraria.ModLoader;

namespace SnekVanity.Content.MiscAccessories;

[AutoloadEquip(EquipType.Waist)]
public sealed class SkirtItem : ModItem
{
	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
	}
}