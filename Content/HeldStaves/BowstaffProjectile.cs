using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.HeldStaves;

public sealed class BowstaffProjectile : ModProjectile
{
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.aiStyle = ProjAIStyleID.SleepyOctopod;
		Projectile.friendly = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.Opacity = 0f;
		Projectile.penetrate = -1;
		Projectile.hide = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 6;
		Projectile.ownerHitCheck = true;
	}

	public override void AI()
	{
		// Vanilla AI (for ProjAIStyle.SleepyOctopod) runs before this and handles some basic held projectile code and owner checks.
		const float useDuration = 75f;
		const float rotationsPerUse = 2f;
		const float useRate = 1f;
		const float spinRange = MathHelper.TwoPi * 0.75f;

		Player owner = Main.player[Projectile.owner];
		ref float progressTimer = ref Projectile.ai[0];
		ref float spinDirection = ref Projectile.ai[1];

		int direction = Math.Sign(Projectile.velocity.X);
		Projectile.velocity = new Vector2(direction, 0f);
		if (progressTimer == 0f)
		{
			Projectile.rotation = new Vector2(direction, -owner.gravDir).ToRotation() + MathHelper.PiOver2 + MathHelper.Pi + MathHelper.PiOver4;
			spinDirection = -direction;
			if (Projectile.velocity.X < 0f)
			{
				spinDirection *= -1f;
				Projectile.rotation -= MathHelper.PiOver2;
			}

			if (owner.gravDir == -1f)
			{
				spinDirection *= -1f;
				Projectile.rotation -= MathHelper.PiOver2;
				if (direction == -1)
				{
					Projectile.rotation -= MathHelper.Pi;
				}
			}
		}

		Projectile.Opacity += 0.5f;
		progressTimer += useRate;
		Projectile.rotation += spinRange * rotationsPerUse / useDuration * direction * spinDirection;
		bool halfwayDone = progressTimer == (int)(useDuration / 2f);
		if (progressTimer >= useDuration || (halfwayDone && !owner.controlUseItem))
		{
			Projectile.Kill();
			owner.reuseDelay = 2;
		}
		else if (halfwayDone)
		{
			int newDirection = (owner.DirectionTo(Main.MouseWorld).X > 0f).ToDirectionInt();
			if (newDirection != Projectile.velocity.X)
			{
				owner.ChangeDir(newDirection);
				Projectile.velocity = new Vector2(newDirection, 0f);
				Projectile.netUpdate = true;
				Projectile.rotation -= MathHelper.Pi;
			}
			else
			{
				spinDirection *= -1f;
			}
		}

		// Already done by the AI Style, but do it again since we update the rotation here.
		owner.itemRotation = MathHelper.WrapAngle(Projectile.rotation);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float headAngle = Projectile.rotation - (MathHelper.PiOver4 * Math.Sign(Projectile.velocity.X));
		float radius = 38f;
		float useless = 0f;
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + (headAngle.ToRotationVector2() * (-radius)), Projectile.Center + (headAngle.ToRotationVector2() * radius), 5f * Projectile.scale, ref useless);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		Player owner = Main.player[Projectile.owner];
		modifiers.HitDirectionOverride = (owner.Center.X < target.Center.X).ToDirectionInt();
	}

	public override void CutTiles()
	{
		float radius = 30f;
		float headAngle = Projectile.rotation - (MathHelper.PiOver4 * Math.Sign(Projectile.velocity.X));
		Utils.PlotTileLine(Projectile.Center + (headAngle.ToRotationVector2() * (-radius)), Projectile.Center + (headAngle.ToRotationVector2() * radius), Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Vector2 drawPosition = Projectile.position + (new Vector2(Projectile.width, Projectile.height) / 2f) + (Vector2.UnitY * Projectile.gfxOffY) - Main.screenPosition;
		SpriteEffects effects = Projectile.spriteDirection != -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, effects);
		return false;
	}
}