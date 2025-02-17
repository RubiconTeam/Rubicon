using Godot.Collections;

namespace Rubicon.View3D;
/// <summary>
/// This class serves as a spawn point for certain character or character group.
/// Determines its position and whether it should be flipped or not.
/// </summary>
[GlobalClass] public partial class SpawnPoint3D : Node3D
{
    /// <summary>
    /// Array of valid character nicknames.
    /// </summary>
    [Export] public StringName[] ValidNicknames = [];
    
    /// <summary>
    /// Spawn point is facing the Z+ axis. 
    /// </summary>
    [Export] public bool FacingZAxis;
    
    /// <summary>
    /// Array of characters to be spawned in the point.
    /// </summary>
    [Export] public Array<Character3D> Characters = [];
    
    /// <summary>
    /// Adds characters and flips them based in <see cref="FacingZAxis"/> and the character's <see cref="Character3D.FacingZAxis"/>.
    /// </summary>
    /// <param name="character">Character to add.</param>
    public void AddCharacter(Character3D character)
    {
        if (Characters.Contains(character))
            return;

        if (FacingZAxis != character.FacingZAxis)
        {
            Vector3 scale = character.Scale;
            scale.Z *= -1f;
            character.Scale = scale;

            character.FlipAnimations = true;
        }
        
        AddChild(character);
        Characters.Add(character);
    }
}
