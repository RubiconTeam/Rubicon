using System.Linq;

namespace Rubicon.View3D;

/// <summary>
/// Basic stage class for <see cref="SpatialSpace"/>.
/// Contains all the spawn points where characters will spawn.
/// </summary>
[GlobalClass] public partial class Stage3D : Node3D
{
    /// <summary>
    /// The default fov for this stage.
    /// </summary>
    [Export] public float Fov = 45;
    
    /// <summary>
    /// Array of spawn points named after each <see cref="CharacterGroup3D"/>'s nickname.
    /// </summary>
    [Export] public SpawnPoint3D[] SpawnPoints = [];

    /// <summary>
    /// Returns a spawn point node with a given nickname.
    /// </summary>
    /// <param name="name">Nickname of the spawn point</param>
    /// <returns><see cref="SpawnPoint3D"/></returns>
    public SpawnPoint3D GetSpawnPoint(StringName name)
    {
        return SpawnPoints.FirstOrDefault(x => x.ValidNicknames.Contains(name));
    }
}
