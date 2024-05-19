using Microsoft.Xna.Framework;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace SnekVanity.Content.DisplayDollMakeup;

internal struct SavedPlayerStyleValues
{
	private const string HAIR_SERIALIZED_KEY = nameof(Hair) + "Serialized";
	private const string HAIR_DYE_SERIALIZED_KEY = nameof(HairDye) + "Serialized";

	internal int SkinVariant { get; private set; }
	internal int Hair { get; private set; }
	internal int HairDye { get; private set; }
	internal Color HairColor { get; private set; }
	internal Color EyeColor { get; private set; }
	internal Color SkinColor { get; private set; }
	internal Color ShirtColor { get; private set; }
	internal Color UndershirtColor { get; private set; }
	internal Color PantsColor { get; private set; }
	internal Color ShoesColor { get; private set; }

	internal SavedPlayerStyleValues(Player player)
	{
		SkinVariant = player.skinVariant;
		Hair = player.hair;
		HairDye = player.hairDye;
		HairColor = player.hairColor;
		EyeColor = player.eyeColor;
		SkinColor = player.skinColor;
		ShirtColor = player.shirtColor;
		UndershirtColor = player.underShirtColor;
		PantsColor = player.pantsColor;
		ShoesColor = player.shoeColor;
	}

	internal SavedPlayerStyleValues(SavedPlayerStyleValues other)
	{
		SkinVariant = other.SkinVariant;
		Hair = other.Hair;
		HairDye = other.HairDye;
		HairColor = other.HairColor;
		EyeColor = other.EyeColor;
		SkinColor = other.SkinColor;
		ShirtColor = other.ShirtColor;
		UndershirtColor = other.UndershirtColor;
		PantsColor = other.PantsColor;
		ShoesColor = other.ShoesColor;
	}

	internal readonly void Retrieve(Player player)
	{
		player.skinVariant = SkinVariant;
		player.hair = Hair;
		player.hairDye = HairDye;
		player.hairColor = HairColor;
		player.eyeColor = EyeColor;
		player.skinColor = SkinColor;
		player.shirtColor = ShirtColor;
		player.underShirtColor = UndershirtColor;
		player.pantsColor = PantsColor;
		player.shoeColor = ShoesColor;
	}

	internal readonly TagCompound SaveData()
	{
		TagCompound tag = new()
		{
			{ nameof(SkinVariant), SkinVariant },
			{ HAIR_SERIALIZED_KEY, SaveHair(Hair) },
			{ HAIR_DYE_SERIALIZED_KEY, SaveHairDye(HairDye) },
			{ nameof(HairColor), HairColor },
			{ nameof(EyeColor), EyeColor },
			{ nameof(SkinColor), SkinColor },
			{ nameof(ShirtColor), ShirtColor },
			{ nameof(UndershirtColor), UndershirtColor },
			{ nameof(PantsColor), PantsColor },
			{ nameof(ShoesColor), ShoesColor }
		};
		return tag;
	}

	internal SavedPlayerStyleValues LoadData(TagCompound tag)
	{
		if (tag is null)
		{
			return this;
		}

		if (tag.TryGet(nameof(SkinVariant), out int skinVariant))
		{
			SkinVariant = skinVariant;
		}

		if (tag.TryGet(HAIR_SERIALIZED_KEY, out string hairSerialized))
		{
			Hair = LoadHair(hairSerialized);
		}
		if (tag.TryGet(nameof(Hair), out int hair))
		{
			Hair = LoadHair(hair.ToString());
		}

		if (tag.TryGet(HAIR_DYE_SERIALIZED_KEY, out string hairDyeSerialized))
		{
			HairDye = LoadHairDye(hairDyeSerialized);
		}
		else if (tag.TryGet(nameof(HairDye), out int hairDye))
		{
			HairDye = LoadHairDye(hairDye.ToString());
		}

		if (tag.TryGet(nameof(HairColor), out Color hairColor))
		{
			HairColor = hairColor;
		}

		if (tag.TryGet(nameof(EyeColor), out Color eyeColor))
		{
			EyeColor = eyeColor;
		}

		if (tag.TryGet(nameof(SkinColor), out Color skinColor))
		{
			SkinColor = skinColor;
		}

		if (tag.TryGet(nameof(ShirtColor), out Color shirtColor))
		{
			ShirtColor = shirtColor;
		}

		if (tag.TryGet(nameof(UndershirtColor), out Color undershirtColor))
		{
			UndershirtColor = undershirtColor;
		}

		if (tag.TryGet(nameof(PantsColor), out Color pantsColor))
		{
			PantsColor = pantsColor;
		}

		if (tag.TryGet(nameof(ShoesColor), out Color shoesColor))
		{
			ShoesColor = shoesColor;
		}

		return this;
	}

	internal readonly void Send(BinaryWriter writer)
	{
		writer.Write((byte)SkinVariant);
		writer.Write7BitEncodedInt(Hair);
		writer.Write7BitEncodedInt(HairDye);
		writer.WriteRGB(HairColor);
		writer.WriteRGB(EyeColor);
		writer.WriteRGB(SkinColor);
		writer.WriteRGB(ShirtColor);
		writer.WriteRGB(UndershirtColor);
		writer.WriteRGB(PantsColor);
		writer.WriteRGB(ShoesColor);
	}

	internal SavedPlayerStyleValues Receive(BinaryReader reader)
	{
		SkinVariant = reader.ReadByte();
		Hair = reader.Read7BitEncodedInt();
		HairDye = reader.Read7BitEncodedInt();
		HairColor = reader.ReadRGB();
		EyeColor = reader.ReadRGB();
		SkinColor = reader.ReadRGB();
		ShirtColor = reader.ReadRGB();
		UndershirtColor = reader.ReadRGB();
		PantsColor = reader.ReadRGB();
		ShoesColor = reader.ReadRGB();
		return this;
	}

	// Need to do extra work for hair and hair dyes since a) ModHair exists and b) Modded hair dyes exist
	// Adapted from PlayerIO::Save/LoadHair() and Save/LoadHairDye().

	private static string SaveHair(int hair)
	{
		return hair < HairID.Count ? hair.ToString() : HairLoader.GetHair(hair).FullName;
	}

	private static int LoadHair(string hair)
	{
		if (int.TryParse(hair, out int vanillaId))
		{
			return vanillaId;
		}

		if (ModContent.TryFind(hair, out ModHair modHair))
		{
			return modHair.Type;
		}

		return 0;
	}

	// PlayerIO::SaveHairDye() uses some internal fields we don't have access to.
	// If only PlayerIO wasn't internal...

	private static string SaveHairDye(int hairDye)
	{
		string playerIoResult = (string)GetPlayerIoMethod("SaveHairDye").Invoke(null, [hairDye]);
		return playerIoResult == string.Empty ? hairDye.ToString() : playerIoResult;
	}

	private static int LoadHairDye(string hairDye)
	{
		if (int.TryParse(hairDye, out int vanillaId))
		{
			return vanillaId;
		}

		if (ModContent.TryFind(hairDye, out ModItem modItem))
		{
			return GameShaders.Hair.GetShaderIdFromItemId(modItem.Type);
		}

		return 0;
	}

	private static MethodInfo GetPlayerIoMethod(string name)
	{
		return typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.IO.PlayerIO", true).GetMethod(name, BindingFlags.Public | BindingFlags.Static);
	}
}