using Godot.Collections;

namespace Rubicon.View3D;
public partial class SpawnPoint3D : Node3D
{
    [Export] public StringName[] ValidNicknames = [];
    [Export] public bool LeftFacing = false;
    [Export] public Array<Character3D> Characters = [];
    public void AddCharacter(Character3D character)
    {
        
    }
}
