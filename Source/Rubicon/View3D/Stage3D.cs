using System.Linq;

namespace Rubicon.View3D;
public partial class Stage3D : Node3D
{
    [Export] public float Fov = 45;
    [Export] public SpawnPoint3D[] SpawnPoints = [];

    public SpawnPoint3D GetSpawnPoint(StringName name)
    {
        return SpawnPoints.FirstOrDefault(x => x.ValidNicknames.Contains(name));
    }
}
