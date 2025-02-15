using Godot.Collections;

namespace Rubicon.View2D;

/// <summary>
/// This class serves as a spawn point for certain character or character group.
/// Determines its position and whether it should be flipped or not.
/// </summary>
[GlobalClass]
public partial class SpawnPoint2D : Node2D
{
    /// <summary>
    /// Array of valid character nicknames.
    /// </summary>
    [Export] public StringName[] ValidNicknames = [];
    
    /// <summary>
    /// Spawn point is facing the Z+ axis. 
    /// </summary>
    [Export] public bool LeftFacing;
    
    /// <summary>
    /// Array of characters to be spawned in the point.
    /// </summary>
    [Export] public Array<Character2D> Characters = [];
    
    /// <summary>
    /// Adds characters and flips them based in <see cref="LeftFacing"/> and the character's <see cref="Character2D.LeftFacing"/>.
    /// </summary>
    /// <param name="character">Character to add.</param>
    public void AddCharacter(Character2D character)
    {
        if (Characters.Contains(character))
            return;
        
        if (LeftFacing != character.LeftFacing)
        {
            Vector2 scale = character.Scale;
            scale.X *= -1f;
            character.Scale = scale;

            character.FlipAnimations = true;
        }
        
        AddChild(character);
        Characters.Add(character);
    }
}