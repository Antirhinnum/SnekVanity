using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SnekVanity.Common.EquipDuplicateAccessories;

[ReinitializeDuringResizeArrays]
public sealed class EquipDuplicateAccessoriesSystem : ModSystem
{
	public static readonly bool[] AccessoryEquippableMultipleTimes = ItemID.Sets.Factory
		.CreateNamedSet(nameof(AccessoryEquippableMultipleTimes))
		.Description("If true for a given item type, then multiple items of that type can be equipped simultaneously.")
		.RegisterBoolSet(defaultState: false);

	public override void Load()
	{
		IL_ItemSlot.AccCheck += BypassIdenticalItemCheck;
		IL_ItemSlot.AccCheck_ForLocalPlayer += BypassIdenticalItemCheck;
	}

	private static void BypassIdenticalItemCheck(ILContext il)
	{
		try
		{
			ILCursor c = new(il);

			// Match (C#):
			//	if (item.IsTheSameAs(itemCollection[i]))
			// Match (IL):
			//	ldloc.s ???
			//	ldelem.ref
			//	callvirt instance bool Terraria.Item::IsTheSameAs(class Terraria.Item)
			//	brfalse.s <failLabel>
			// Change to (C#):
			//	if (item.IsTheSameAs(itemCollection[i]) && !AllowDuplicateEquips(item.type))
			// Change to (IL):
			//	ldloc.s ???
			//	ldelem.ref
			//	callvirt instance bool Terraria.Item::IsTheSameAs(class Terraria.Item)
			//	brfalse.s <failLabel>
			//	ldarg.1
			//	call bool AllowDuplicateEquips(class Terraria.Item)
			//	brtrue.s <failLabel>
			ILLabel failLabel = null;
			c.GotoNext(MoveType.After,
					i => i.MatchLdloc(out _),
					i => i.MatchLdelemRef(),
					i => i.MatchCallOrCallvirt<Item>("IsTheSameAs"),
					i => i.MatchBrfalse(out failLabel));

			c.EmitLdarg1();
			c.EmitCall(typeof(EquipDuplicateAccessoriesSystem).GetMethod(nameof(AllowDuplicateEquips),
				BindingFlags.Static | BindingFlags.NonPublic));
			c.EmitBrtrue(failLabel);
		}
		catch
		{
			SnekVanity mod = ModContent.GetInstance<SnekVanity>();
			MonoModHooks.DumpIL(mod, il);
			mod.Logger.Error($"Failed to patch in {nameof(EquipDuplicateAccessoriesSystem)}!");
		}
	}

	private static bool AllowDuplicateEquips(Item item)
	{
		return AccessoryEquippableMultipleTimes[item.type];
	}
}