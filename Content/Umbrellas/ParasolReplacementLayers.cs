using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Content.Umbrellas;

public abstract class ParasolReplacementLayer : PlayerDrawLayer
{
	private const string _parasolModName = "Parasol";
	private static Mod _parasolMod;
	private static MethodInfo _parasolDrawUmbrellaMethod;
	private static Item _dummyUmbrella;

	protected abstract string ParasolDrawLayerName { get; }
	protected abstract bool TopLayer { get; }

	public override sealed bool IsLoadingEnabled(Mod mod)
	{
		return ModLoader.TryGetMod(_parasolModName, out _parasolMod);
	}

	public override sealed Position GetDefaultPosition()
	{
		return _parasolMod.TryFind(ParasolDrawLayerName, out PlayerDrawLayer parasolLayer)
			? new AfterParent(parasolLayer)
			: throw new Exception("Couldn't find the Parasol layer to put this layer behind!");
	}

	public override sealed bool GetDefaultVisibility(PlayerDrawSet drawInfo)
	{
		// Parasol uses Player::HeldItem instead of PlayerDrawSet::heldItem, so mirror that here.
		return drawInfo.drawPlayer.HeldItem.type != ItemID.FairyQueenMagicItem && drawInfo.drawPlayer.TryGetModPlayer(out ParasolHijackPlayer parasolPlayer) && parasolPlayer.HasEquippedUmbrellaAndNotWearingParasolHandledUmbrella;
	}

	protected override sealed void Draw(ref PlayerDrawSet drawInfo)
	{
		if (_parasolDrawUmbrellaMethod is null)
		{
			PlayerDrawLayer drawUmbrellaLayer = _parasolMod.Find<PlayerDrawLayer>("DrawUmbrella");
			_parasolDrawUmbrellaMethod = drawUmbrellaLayer.GetType().GetMethod("DU", BindingFlags.Public | BindingFlags.Static);
		}

		_dummyUmbrella ??= new Item(ItemID.Umbrella);

		// The easiest way to draw the same way that Parasol does is to just call its draw method.
		// It adds data if one of the player's accessories is the Umbrella or Tragic Umbrella.
		// Unfortunately, the mod isn't coded very well, so it checks accessory slots directly.
		int oldDataCount = drawInfo.DrawDataCache.Count;
		Player player = drawInfo.drawPlayer;
		Item oldAccessory = player.armor[19];
		player.armor[19] = _dummyUmbrella;
		_parasolDrawUmbrellaMethod.Invoke(null, new object[] { drawInfo, TopLayer });
		player.armor[19] = oldAccessory;

		// DU should have added a single DrawData. Replace the texture, anything based on it (origin), and the dye.
		if (drawInfo.DrawDataCache.Count > oldDataCount)
		{
			DrawData data = drawInfo.DrawDataCache[^1];
			ParasolHijackPlayer parasolPlayer = player.GetModPlayer<ParasolHijackPlayer>();
			Texture2D texture = GetParasolTexture(parasolPlayer.equippedUmbrella);
			data.texture = texture;
			data.origin = new Vector2(texture.Width * 0.5f, player.gravDir == -1 ? 10 : texture.Height - 10);
			data.shader = parasolPlayer.cUmbrella;
			drawInfo.DrawDataCache[^1] = data;
		}
	}

	private Texture2D GetParasolTexture(Item item)
	{
		return ModContent.Request<Texture2D>(item.ModItem.Texture + (TopLayer ? "_Top" : "_Bottom")).Value;
	}
}

public sealed class ParasolBottomLayer : ParasolReplacementLayer
{
	protected override string ParasolDrawLayerName { get; } = "DrawUmbrella";
	protected override bool TopLayer { get; } = false;
}

public sealed class ParasolTopLayer : ParasolReplacementLayer
{
	protected override string ParasolDrawLayerName { get; } = "DrawUmbrellaTop";
	protected override bool TopLayer { get; } = true;
}