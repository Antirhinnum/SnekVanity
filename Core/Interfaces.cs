using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SnekVanity.Core;

#region Asymmetric Equips Support

/// <summary>
/// A <see cref="ModItem"/> that implements this interface will use glove asymmetry when the mod "Asymmetric Equips" is enabled.
/// </summary>
public interface IAmAsymmetricGlove
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will use manual asymmetry when the mod "Asymmetric Equips" is enabled.
/// </summary>
public interface IAmAsymmetricSpecial
{ }

#endregion Asymmetric Equips Support

#region Player Dyes

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's head skin color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.Head"/>.
/// </summary>
public interface IDyeHeadSkin
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's eye white color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.EyeWhites"/>.
/// </summary>
public interface IDyeEyeWhites
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's eye color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.Eyes"/>.
/// </summary>
public interface IDyeEyes
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's torso skin color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.TorsoSkin"/>.
/// </summary>
public interface IDyeTorsoSkin
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change th player's undershirt color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.Undershirt"/> and <see cref="PlayerTextureID.Undershirt"/>.
/// </summary>
public interface IDyeUndershirt
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's hand skin color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.Hands"/> and <see cref="PlayerTextureID.ArmHand"/>.
/// </summary>
public interface IDyeHandSkin
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's shirt color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.Shirt"/>, <see cref="PlayerTextureID.ArmShirt"/>, and <see cref="PlayerTextureID.Extra"/>.
/// </summary>
public interface IDyeShirt
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's arm skin color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.ArmSkin"/>.
/// </summary>
public interface IDyeArmSkin
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's leg skin color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.LegSkin"/>.
/// </summary>
public interface IDyeLegSkin
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's pants color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.Pants"/>.
/// </summary>
public interface IDyePants
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's shoe color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.Shoes"/>.
/// </summary>
public interface IDyeShoes
{ }

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will change the player's eyelid color when dyed.<br/>
/// Affects <see cref="PlayerTextureID.EyeBlink"/>.
/// </summary>
public interface IDyeEyeBlink
{ }

#endregion Player Dyes

/// <summary>
/// Any <see cref="ModItem"/> that implements this interface will force the player's eyes into a certain state when equipped.
/// </summary>
public interface IForceEyeState
{
	/// <summary>
	/// Sets a player's eye state.
	/// </summary>
	/// <param name="player">The player whose eye state is being set.</param>
	/// <param name="oldFrame">The eye state the player was in before this method was called.</param>
	/// <returns>The new eye state of the player.</returns>
	EyeFrame SetEyeState(Player player, EyeFrame oldFrame);
}

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
	/// The condition under which this item is sold.
	/// </summary>
	bool Available => true;
}