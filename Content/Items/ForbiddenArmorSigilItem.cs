using SnekVanity.Common.Hooks;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class ForbiddenArmorSigilItem : ModItem
{
	public sealed class ForbiddenArmorSigilPlayer : ModPlayer, IAddDyeSlots
	{
		public bool sigilActive;
		public int cSigil;

		public override void ResetEffects()
		{
			sigilActive = false;
		}

		public void ClearDyeSlots()
		{
			cSigil = 0;
		}

		public void UpdateDyeSlots(Item armorItem, Item dyeItem)
		{
			if (armorItem.ModItem is ForbiddenArmorSigilItem)
			{
				cSigil = dyeItem.dye;
			}
		}
	}

	public sealed class ForbiddenArmorSigilPlayerLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition()
		{
			return new AfterParent(PlayerDrawLayers.ForbiddenSetRing);
		}

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player player = drawInfo.drawPlayer;
			ForbiddenArmorSigilPlayer forbiddenPlayer = player.GetModPlayer<ForbiddenArmorSigilPlayer>();
			if (!forbiddenPlayer.sigilActive)
			{
				return;
			}

			if (!player.setForbidden)
			{
				// The sigil isn't present, so add it.
				int oldCBody = drawInfo.cBody;
				drawInfo.cBody = forbiddenPlayer.cSigil;
				player.setForbidden = true;

				PlayerDrawLayers.DrawPlayer_05_ForbiddenSetRing(ref drawInfo);

				drawInfo.cBody = oldCBody;
				player.setForbidden = false;
			}
			else if (forbiddenPlayer.cSigil > 0)
			{
				// The sigil *is* present, so dye it if needed.
				for (int i = drawInfo.DrawDataCache.Count - 1; i >= 0; i--)
				{
					DrawData data = drawInfo.DrawDataCache[i];
					if (data.texture == TextureAssets.Extra[ExtrasID.ForbiddenSign].Value)
					{
						data.shader = forbiddenPlayer.cSigil;
						drawInfo.DrawDataCache[i] = data;
						break;
					}
				}
			}
		}
	}

	public override string Texture => $"Terraria/Images/Extra_{ExtrasID.ForbiddenSign}";

	public override void SetDefaults()
	{
		Item.DefaultToAccessory();
		Item.vanity = true;
		Item.hasVanityEffects = true;
		Item.value = Item.sellPrice(gold: 2);
	}

	public override void UpdateAccessory(Player player, bool hideVisual)
	{
		if (!hideVisual)
		{
			player.GetModPlayer<ForbiddenArmorSigilPlayer>().sigilActive = true;
		}
	}

	public override void UpdateVanity(Player player)
	{
		player.GetModPlayer<ForbiddenArmorSigilPlayer>().sigilActive = true;
	}

	public override void AddRecipes()
	{
		Recipe firstRecipe = CreateRecipe()
			.AddIngredient(ItemID.AncientBattleArmorMaterial)
			.AddIngredient(ItemID.AdamantiteBar, 4)
			.SortAfter(Main.recipe.First(r => r.HasIngredient(ItemID.TitaniumBar) && r.HasResult(ItemID.AncientBattleArmorPants))) // Try to ensure this is after vanilla's recipe and not some alternative mod recipe
			.Register();

		CreateRecipe()
			.AddIngredient(ItemID.AncientBattleArmorMaterial)
			.AddIngredient(ItemID.TitaniumBar, 4)
			.SortAfter(firstRecipe)
			.DisableDecraft()
			.Register();
	}
}