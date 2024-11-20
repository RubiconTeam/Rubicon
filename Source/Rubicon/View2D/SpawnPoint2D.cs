using Godot.Collections;

namespace Rubicon.View2D;

[GlobalClass]
public partial class SpawnPoint2D : Node2D
{
    [Export] public bool LeftFacing = false;

    [Export] public Array<Character2D> Characters = new();

    public void AddCharacter(Character2D character)
    {
        if (Characters.Contains(character))
            return;
        
        if (LeftFacing != character.LeftFacing)
        {
            Vector2 scale = character.Scale;
            scale.X *= -1f;
            character.Scale = scale;
        }
        
        AddChild(character);
        Characters.Add(character);
    }
}