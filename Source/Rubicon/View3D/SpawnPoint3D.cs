using Godot.Collections;

namespace Rubicon.View3D;
[GlobalClass] public partial class SpawnPoint3D : Node3D
{
    [Export] public StringName[] ValidNicknames = [];
    [Export] public bool FrontFacing;
    [Export] public Array<Character3D> Characters = [];
    public void AddCharacter(Character3D character)
    {
        if (Characters.Contains(character))
            return;

        if (FrontFacing != character.FacingZAxis)
        {
            Vector3 scale = character.Scale;
            scale.Z *= -1f;
            character.Scale = scale;

            character.FlipAnimations = false;
        }
        
        AddChild(character);
        Characters.Add(character);
    }
}
