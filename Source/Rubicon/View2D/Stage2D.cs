namespace Rubicon.View2D;

public partial class Stage2D : Node2D
{
    [Export] public bool SnapZoomOnStart = true;
    
    [Export] public Vector2 Zoom = Vector2.One;

    [Export] public SpawnPoint2D[] SpawnPoints = [];
}