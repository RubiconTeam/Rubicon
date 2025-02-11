
namespace Rubicon.View3D;
public partial class RubiconCamera3D : Camera3D
{
    [Export] public Vector3 TargetPosition = Vector3.Zero;
    [Export] public Vector3 OffsetPosition = Vector3.Zero;
    [Export] public Vector3 TargetRotation = Vector3.Zero;
    [Export] public Vector3 OffsetRotation = Vector3.Zero;
    [Export] public float TargetFov = 45f;
    [Export] public float OffsetFov;
    
}
