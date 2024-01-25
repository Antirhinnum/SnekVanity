using Microsoft.Xna.Framework;
using SnekVanity.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace SnekVanity.Content.Items;

public sealed class JapaneseUmbrellaItem : ModItem, IEquippableParasol
{
	public override LocalizedText Tooltip => ModLoader.HasMod("Parasol") ? this.GetLocalization("TooltipParasol") : this.GetLocalization("TooltipStandalone");

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.Umbrella);
	}

	public bool ActivatesSlowFall(Player player)
	{
		return player.velocity.Y > 0f;
	}

	public override void HoldStyle(Player player, Rectangle heldItemFrame)
	{
		if (Item.holdStyle == ItemHoldStyleID.None)
		{
			return;
		}

		player.itemRotation = 0f;
		player.itemLocation.X = player.position.X + (player.width * 0.5f) - (36f * player.direction);
		player.itemLocation.Y = player.position.Y + 22f + player.HeightOffsetHitboxCenter;
		player.fallStart = (int)(player.position.Y / 16f);
		if (player.gravDir == -1f)
		{
			player.itemRotation = 0f - player.itemRotation;
			player.itemLocation.Y = player.position.Y + player.height + (player.position.Y - player.itemLocation.Y);
			if (player.velocity.Y < -2f && !player.controlDown)
			{
				player.velocity.Y = -2f;
			}
		}
		else if (player.velocity.Y > 2f && !player.controlDown)
		{
			player.velocity.Y = 2f;
		}
	}

	public override void HoldItemFrame(Player player)
	{
		if (Item.holdStyle == ItemHoldStyleID.None)
		{
			return;
		}
		if (player.sandStorm && player.miscCounter % 4 == 0)
		{
			player.itemLocation.X = player.position.X + (player.width * 0.5f) - (36f * player.direction);
		}
	}

	public override void UseStyle(Player player, Rectangle heldItemFrame)
	{
		if (Item.useStyle == ItemUseStyleID.None)
		{
			return;
		}

		if (player.itemAnimation > player.itemAnimationMax * 0.666)
		{
			player.itemLocation.X = -1000f;
			player.itemLocation.Y = -1000f;
			player.itemRotation = -1.3f * player.direction;
		}
		else
		{
			player.itemLocation.X = player.position.X + (player.width * 0.5f) + (((heldItemFrame.Width * 0.5f) - 4f) * player.direction);
			player.itemLocation.Y = player.position.Y + 24f + player.HeightOffsetHitboxCenter;
			float stabOffset = (player.itemAnimation / (float)player.itemAnimationMax * heldItemFrame.Width * player.direction * player.GetAdjustedItemScale(Item) * 1.2f) - (10f * player.direction);
			if (stabOffset > -4f && player.direction == -1)
				stabOffset = -8f;

			if (stabOffset < 4f && player.direction == 1)
				stabOffset = 8f;

			player.itemLocation.X -= stabOffset;
			player.itemRotation = 0.8f * player.direction;
			player.itemLocation.X -= 6f * player.direction;
		}

		if (player.gravDir == -1f)
		{
			player.itemRotation = 0f - player.itemRotation;
			player.itemLocation.Y = player.position.Y + player.height + (player.position.Y - player.itemLocation.Y);
		}

		if (player.itemRotation != 0f)
		{
			player.itemLocation.Y -= 44f * player.gravDir;
			player.itemRotation += MathHelper.PiOver4 * player.direction * player.gravDir;
		}
	}

	public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox)
	{
		if (Item.useStyle == ItemUseStyleID.None)
		{
			return;
		}

		if (player.itemAnimation > player.itemAnimationMax * 0.666)
		{
			return;
		}

		hitbox.Height += 28;
		hitbox.Width -= 20;
		if (player.direction == -1)
		{
			hitbox.X += 20;
		}
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.DynastyWood, 12)
			.AddIngredient(ItemID.Firefly, 3)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}