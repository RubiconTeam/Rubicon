using Godot;

namespace Rubicon.Space2D;

public partial class SpawnPoint2D : Node2D
{
    /// <summary>
    /// An array of spawn points for characters to spawn at. Characters will spawn at the index according to their spawn point index.
    /// </summary>
    [Export] public Node2D[] Spawns;
    
    /// <summary>
    /// Whether this spawn point is supposed to be facing left.
    /// </summary>
    [Export] public bool LeftFacing = false;
    
    /// <summary>
    /// Since ParallaxLayer might change the Position, this is used for reference for the Camera.
    /// </summary>
    [Export] public Vector2 StartingPosition = Vector2.Zero;
}