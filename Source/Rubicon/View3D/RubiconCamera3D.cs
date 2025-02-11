
using Rubicon.Data;

namespace Rubicon.View3D;
public partial class RubiconCamera3D : Camera3D
{
    [Export] public Vector3 TargetPosition = Vector3.Zero;
    [Export] public Vector3 OffsetPosition = Vector3.Zero;
    
    [Export] public Vector3 TargetRotation = Vector3.Zero;
    [Export] public Vector3 OffsetRotation = Vector3.Zero;
    
    [Export] public float TargetFov = 45f;
    [Export] public float OffsetFov;
    
    [ExportGroup("Update Settings"), Export] public CameraUpdate PositionUpdateType = CameraUpdate.Interpolation;
    [Export] public CameraUpdate RotationUpdateType = CameraUpdate.Interpolation;
    [Export] public CameraUpdate FovUpdateType = CameraUpdate.Interpolation;
    
    [ExportSubgroup("Interpolation"), Export] public float PositionLerpWeight = 2.4f;
    [Export] public float RotationLerpWeight = 2.4f;
    [Export] public float FovLerpWeight = 3.125f;
    
    [ExportSubgroup("Tweening"), Export] public float PositionTweenDuration = 1f;
    [Export] public float RotationLerpDuration = 1f;
    [Export] public float FovTweenDuration = 1f;
    
    private Vector3 _previousPosition = Vector3.Zero;
    private Vector3 _previousRotation = Vector3.Zero;
    private float _previousZoom;

    private Tween _posTween;
    private Tween _rotTween;
    private Tween _fovTween;

    public override void _Process(double delta)
    {
        float deltaF = (float)delta;
        UpdatePosition(deltaF);
        UpdateRotation(deltaF);
        UpdateFov(deltaF);
    }

    public virtual void UpdatePosition(float delta)
    {
        Vector3 finalPosition = TargetPosition + OffsetPosition;
        switch (PositionUpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Tween:
                if (_previousPosition != finalPosition && !_posTween.IsRunning())
                    TweenPosition(finalPosition, PositionTweenDuration, true);
                break;
            case CameraUpdate.Interpolation:
                Vector3 pos = GlobalPosition;
                if (pos.IsEqualApprox(finalPosition))
                    return;
                
                Vector3 nextPos = pos.Lerp(finalPosition, PositionLerpWeight * delta);
                if (nextPos.IsEqualApprox(finalPosition))
                {
                    GlobalPosition = finalPosition;
                    return;
                }

                GlobalPosition = nextPos;
                break;
        }
    }
    
    public virtual void UpdateRotation(float delta)
    {
        Vector3 finalRotation = TargetRotation + OffsetRotation;
        switch (RotationUpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Tween:
                if (_previousPosition != finalRotation && !_posTween.IsRunning())
                    TweenRotation(finalRotation, PositionTweenDuration, true);
                break;
            case CameraUpdate.Interpolation:
                Vector3 rot = GlobalRotation;
                if (rot.IsEqualApprox(finalRotation))
                    return;
                
                Vector3 nextRot = rot.Lerp(finalRotation, RotationLerpWeight * delta);
                if (nextRot.IsEqualApprox(finalRotation))
                {
                    GlobalRotation = finalRotation;
                    return;
                }

                GlobalRotation = nextRot;
                break;
        }
    }
    
    public virtual void UpdateFov(float delta)
    {
        float finalFov = TargetFov + OffsetFov;
        switch (FovUpdateType)
        {
            case CameraUpdate.Instant:
                Fov = finalFov;
                break;
            case CameraUpdate.Interpolation:
                float fov = Fov;
                if (Mathf.IsEqualApprox(fov,finalFov))
                    return;
                
                float nextZoom = Mathf.Lerp(fov, finalFov, FovLerpWeight * delta);
                if (Mathf.IsEqualApprox(fov,finalFov))
                {
                    Fov = finalFov;
                    return;
                }

                Fov = nextZoom;
                break;
            case CameraUpdate.Tween:
                if (_previousZoom != finalFov && !_fovTween.IsRunning())
                    TweenFov(finalFov, FovTweenDuration, true);
                break;
        }
    }
    
    /// <summary>
    /// Tween the position of this camera to the final position.
    /// </summary>
    /// <param name="position">The final position</param>
    /// <param name="duration">How long the tween will last</param>
    /// <param name="force">Setting this to true will immediately kill the previous, otherwise this will queue this tween so that it is ran when the previous one is finished.</param>
    public void TweenPosition(Vector3 position, double duration, bool force = false)
    {
        if (_posTween == null || (_posTween != null && _posTween.IsRunning() && force))
        {
            _posTween?.Kill();
            _posTween = CreateTween();
        }
        
        _posTween.TweenProperty(this, property: "global_position", position, duration);
    }
    
    /// <summary>
    /// Tween the position of this camera to the final position.
    /// </summary>
    /// <param name="rotation">The final position</param>
    /// <param name="duration">How long the tween will last</param>
    /// <param name="force">Setting this to true will immediately kill the previous, otherwise this will queue this tween so that it is ran when the previous one is finished.</param>
    public void TweenRotation(Vector3 rotation, double duration, bool force = false)
    {
        if (_rotTween == null || (_rotTween != null && _rotTween.IsRunning() && force))
        {
            _rotTween?.Kill();
            _rotTween = CreateTween();
        }
        
        _rotTween.TweenProperty(this, property: "global_rotation", rotation, duration);
    }

    /// <summary>
    /// Tween the fov of this camera to the fov specified.
    /// </summary>
    /// <param name="fov">The fov specified</param>
    /// <param name="duration">How long the tween will last</param>
    /// <param name="force">Setting this to true will immediately kill the previous, otherwise this will queue this tween so that it is ran when the previous one is finished.</param>
    public void TweenFov(float fov, double duration, bool force = false)
    {
        if (_fovTween == null || (_fovTween != null && _fovTween.IsRunning() && force))
        {
            _fovTween?.Kill();
            _fovTween = CreateTween();
        }
        
        _fovTween.TweenProperty(this, property: "fov", fov, duration);
    }
}
