using System.Linq;

namespace Rubicon.View2D;

/// <summary>
/// Basic stage class for <see cref="CanvasItemSpace"/>.
/// Contains all the spawn points where characters will spawn.
/// </summary>
[GlobalClass]
public partial class Stage2D : Node2D
{
    /// <summary>
    /// The default zoom for this stage.
    /// </summary>
    [Export] public Vector2 Zoom = Vector2.One;

    /// <summary>
    /// Array of spawn points named after each <see cref="CharacterGroup2D"/>'s nickname.
    /// </summary>
    [Export] public SpawnPoint2D[] SpawnPoints = [];

    /// <summary>
    /// Returns a spawn point node with a given nickname.
    /// </summary>
    /// <param name="name">Nickname of the spawn point</param>
    /// <returns><see cref="SpawnPoint2D"/></returns>
    public SpawnPoint2D GetSpawnPoint(StringName name)
    {
        return SpawnPoints.FirstOrDefault(x => x.ValidNicknames.Contains(name));
    }
}