using System.Linq;

namespace Rubicon.View2D;

[GlobalClass]
public partial class Stage2D : Node2D
{
    [Export] public Vector2 Zoom = Vector2.One;

    [Export] public SpawnPoint2D[] SpawnPoints = [];

    public SpawnPoint2D GetSpawnPoint(StringName name)
    {
        return SpawnPoints.FirstOrDefault(x => x.ValidNicknames.Contains(name));
    }
}