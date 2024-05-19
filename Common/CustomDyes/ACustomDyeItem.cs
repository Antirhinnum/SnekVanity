using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Common.CustomDyes;

/// <summary>
/// A base class for dyes that need per-item data to function.
/// </summary>
public abstract class ACustomDyeItem : ModItem
{
	private static Asset<Effect> _pixelShaderAsset;

	internal int cachedDataIndex;

	/// <summary>
	/// The unique registered shader index of this item's shader.
	/// </summary>
	protected internal int UniqueShaderIndex { get; private set; } = -1;

	/// <summary>
	/// The <see cref="ArmorShaderData"/> associated with this item.
	/// </summary>
	protected ArmorShaderData ShaderData { get; private set; }

	/// <summary>
	/// Determines if this dye has any active effects / should do anything.
	/// <br/> If <see langword="false"/>, anything drawn with this item's <see cref="Item.dye"/> value will be given no shader.
	/// </summary>
	public abstract bool HasAnyEffects { get; }

	public ACustomDyeItem()
	{
		CustomDyeHandler.CacheItem(this);
	}

	~ACustomDyeItem()
	{
		CustomDyeHandler.UncacheItem(this);
	}

	public override void Unload()
	{
		_pixelShaderAsset = null;
	}

	public override void SetStaticDefaults()
	{
		if (!Main.dedServ)
		{
			_pixelShaderAsset ??= Main.Assets.Request<Effect>("PixelShader");
			GameShaders.Armor.BindShader(Type, BaseEffect(_pixelShaderAsset));
		}
	}

	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.RedDye);
		Item.maxStack = 1;
		Item.dye = GetItemDyeValue();
		CustomDyeHandler.CacheItem(this);
	}

	/// <summary>
	/// Generates the base <see cref="ArmorShaderData"/> that this dye will be based on.
	/// <br/> By default, generates a shader that does nothing.
	/// </summary>
	/// <param name="pixelShaderAsset"><em>Terraria</em>'s pixel shader as an <see cref="Asset{T}"/>.</param>
	protected virtual ArmorShaderData BaseEffect(Asset<Effect> pixelShaderAsset)
	{
		return new ArmorShaderData(pixelShaderAsset, "Default");
	}

	// Item::dye is the canonical field to pull from for dyeing anything.
	// Individual dye values are limited to the range [0, 4000) by PlayerDrawHelper::(Un)PackShader().
	// Therefore, we can use values >= 4000 to signify that additional data is packed.
	// We just need to ensure that the packed value is never actually used as a shader index.
	protected internal int GetItemDyeValue()
	{
		if (UniqueShaderIndex == -1)
		{
			int v = GameShaders.Armor.GetShaderIdFromItemId(Type);
			if (v != 0)
			{
				UniqueShaderIndex = v;
				ShaderData = GameShaders.Armor.GetShaderFromItemId(Type);
			}
		}

		return UniqueShaderIndex == -1
			? 0
			: (int)(((uint)UniqueShaderIndex << 16) | (ushort)cachedDataIndex);
	}

	/// <summary>
	/// Modify the texture, color, and shader used when drawing under the effects of this item's dye.
	/// </summary>
	/// <param name="texture">The texture being drawn.</param>
	/// <param name="color">The color being drawn in.</param>
	/// <param name="shader">The shader being used, originally set to <see cref="UniqueShaderIndex"/>.</param>
	/// <param name="associatedPlayer">The <see cref="Player"/> associated with this drawing.</param>
	/// <param name="sourceRectangle">The frame of the texture being drawn.</param>
	public abstract void ModifyDrawData(ref Texture2D texture, ref Color color, ref int shader, Player associatedPlayer, Rectangle? sourceRectangle = null);
}