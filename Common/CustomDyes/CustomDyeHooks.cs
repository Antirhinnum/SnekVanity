using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace SnekVanity.Common.CustomDyes;

internal static class CustomDyeHooks
{
	// Called from the main Mod constructor.
	// If these hooks aren't done ASAP, then some Main::EntitySpriteDraw() calls get inlined -- notably, for projectile drawing.
	// This breaks the DIY Dye when used with Pet Shampoo or other projectile-dyeing items.1
	internal static void DoSuperEarlyHooks()
	{
		On_PlayerDrawLayers.DrawPlayer_RenderAllLayers += ReplacePlayerTextures;
		On_Main.EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_float_SpriteEffects_float += ReplaceEntityTexturesFloatScale;
		On_Main.EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_Vector2_SpriteEffects_float += ReplaceEntityTexturesVector2Scale;
		On_Main.EntitySpriteDraw_DrawData += ReplaceEntityTexturesDrawData;
	}

	public static void ReplacePlayerTextures(On_PlayerDrawLayers.orig_DrawPlayer_RenderAllLayers orig, ref PlayerDrawSet drawinfo)
	{
		for (int i = 0; i < drawinfo.DrawDataCache.Count; i++)
		{
			DrawData data = drawinfo.DrawDataCache[i];
			ModifyDrawData(ref data.texture, ref data.color, ref data.shader, drawinfo.drawPlayer, data.sourceRect);
			drawinfo.DrawDataCache[i] = data;
		}

		orig(ref drawinfo);
	}

	public static void ReplaceEntityTexturesFloatScale(On_Main.orig_EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_float_SpriteEffects_float orig, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float worthless)
	{
		int originalShader = Main.CurrentDrawnEntityShader;
		TryReplacingTextureAndEntityShader(ref texture, ref color, sourceRectangle);
		orig(texture, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
		Main.CurrentDrawnEntityShader = originalShader;
	}

	public static void ReplaceEntityTexturesVector2Scale(On_Main.orig_EntitySpriteDraw_Texture2D_Vector2_Nullable1_Color_float_Vector2_Vector2_SpriteEffects_float orig, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float worthless)
	{
		int originalShader = Main.CurrentDrawnEntityShader;
		TryReplacingTextureAndEntityShader(ref texture, ref color, sourceRectangle);
		orig(texture, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
		Main.CurrentDrawnEntityShader = originalShader;
	}

	public static void ReplaceEntityTexturesDrawData(On_Main.orig_EntitySpriteDraw_DrawData orig, DrawData data)
	{
		int originalShader = Main.CurrentDrawnEntityShader;
		TryReplacingTextureAndEntityShader(ref data.texture, ref data.color, data.sourceRect);
		orig(data);
		Main.CurrentDrawnEntityShader = originalShader;
	}

	private static void TryReplacingTextureAndEntityShader(ref Texture2D texture, ref Color color, Rectangle? sourceRectangle)
	{
		Player suppliedPlayer = null;
		if (Main.CurrentDrawnEntity is Player player)
		{
			suppliedPlayer = player;
		}
		else if (Main.CurrentDrawnEntity is Projectile projectile)
		{
			suppliedPlayer = Main.player[projectile.owner];
		}

		ModifyDrawData(ref texture, ref color, ref Main.CurrentDrawnEntityShader, suppliedPlayer, sourceRectangle);
	}

	internal static void ModifyDrawData(ref Texture2D texture, ref Color color, ref int shader, Player player, Rectangle? sourceRectangle)
	{
		if (!CustomDyeHandler.TryGetDyeFromShaderIndex(shader, out ACustomDyeItem customDye))
		{
			return;
		}

		if (!customDye.HasAnyEffects)
		{
			shader = 0;
			return;
		}

		customDye.CacheSelf();

		Texture2D originalTexture = texture;
		shader = customDye.UniqueShaderIndex;
		customDye.ModifyDrawData(ref texture, ref color, ref shader, player, sourceRectangle);
		if (texture != originalTexture)
		{
			texture.Tag = originalTexture;
		}

		// If the shader has been changed to a custom dye, handle it as well.
		ModifyDrawData(ref texture, ref color, ref shader, player, sourceRectangle);
	}
}