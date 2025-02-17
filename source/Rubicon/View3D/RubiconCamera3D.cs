
using Rubicon.Data;

namespace Rubicon.View3D;

/// <summary>
/// <see cref="Camera3D"/> utility that manages smooth positioning, tweening and fov changes. 
/// </summary>

public partial class RubiconCamera3D : Camera3D
{
    /// <summary>
    /// The final position to smoothly move to.
    /// </summary>
    [Export] public Vector3 TargetPosition = Vector3.Zero;
    
    /// <summary>
    /// Offset added to <see cref="TargetPosition"/> when calculating a new position.
    /// Similar to <see cref="Camera2D.Offset"/>, except it also gets smoothed.
    /// </summary>
    [Export] public Vector3 OffsetPosition = Vector3.Zero;
    
    /// <summary>
    /// The final rotation to smoothly move to.
    /// </summary>
    [Export] public Vector3 TargetRotation = Vector3.Zero;
    
    /// <summary>
    /// Offset added to <see cref="TargetRotation"/> when calculating a new rotation.
    /// </summary>
    [Export] public Vector3 OffsetRotation = Vector3.Zero;
    
    /// <summary>
    /// The final fov to smoothly move to.
    /// </summary>
    [Export] public float TargetFov = 0f;
    
    /// <summary>
    /// Offset added to <see cref="TargetFov"/> when calculating a new fov value.
    /// </summary>
    [Export] public float OffsetFov = 0f;
    
    /// <summary>
    /// Values used when updating the camera's position.
    /// </summary>
    [ExportGroup("Motion Settings"), Export] public CameraMotionData PositionMotionData = new();
    
    /// <summary>
    /// Values used when updating the camera's rotation.
    /// </summary>
    [Export] public CameraMotionData RotationMotionData = new();
    
    /// <summary>
    /// Values used when updating the camera's fov.
    /// </summary>
    [Export] public CameraMotionData FovMotionData = new();
    
    private Vector3 _previousPosition = Vector3.Zero;
    private Vector3 _previousRotation = Vector3.Zero;
    private float _previousFov = 0;

    private Tween _posTween;
    private Tween _rotTween;
    private Tween _fovTween;

    public override void _Process(double delta)
    {
        if (PositionMotionData == null && RotationMotionData == null && FovMotionData == null)
            return;
        
        float deltaF = (float)delta;
        UpdatePosition(deltaF);
        UpdateRotation(deltaF);
        UpdateFov(deltaF);
    }
    
    /// <summary>
    /// Updates the camera's position depending on the position's <see cref="CameraMotionData"/>.
    /// </summary>
    public virtual void UpdatePosition(float delta)
    {
        Vector3 finalPosition = TargetPosition + OffsetPosition;
        switch (PositionMotionData.UpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Smoothing:
                GlobalPosition = finalPosition;
                break;
            case CameraUpdate.Tween:
                if (_previousPosition != finalPosition && !_posTween.IsRunning())
                    MotionTween(ref _posTween, "global_position", finalPosition, PositionMotionData, true);
                break;
            case CameraUpdate.Interpolation:
                GlobalPosition = MotionInterpolation(GlobalPosition, finalPosition, PositionMotionData.LerpWeight, delta);
                break;
        }
    }
    
    /// <summary>
    /// Updates the camera's rotation depending on the rotation's <see cref="CameraMotionData"/>.
    /// </summary>
    public virtual void UpdateRotation(float delta)
    {
        Vector3 finalRotation = TargetRotation + OffsetRotation;
        switch (RotationMotionData.UpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Smoothing:
                RotationMotionData.UpdateType = CameraUpdate.Interpolation;
                break;
            case CameraUpdate.Tween:
                if (_previousRotation != finalRotation && !_rotTween.IsRunning())
                    MotionTween(ref _rotTween, "global_rotation", finalRotation, RotationMotionData, true);
                break;
            case CameraUpdate.Interpolation:
                GlobalRotation = MotionInterpolation(GlobalRotation, finalRotation, RotationMotionData.LerpWeight, delta);
                break;
        }
    }
    
    /// <summary>
    /// Updates the camera's fov depending on the fov's <see cref="CameraMotionData"/>.
    /// </summary>
    public virtual void UpdateFov(float delta)
    {
        float finalFov = TargetFov + OffsetFov;
        switch (FovMotionData.UpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Smoothing:
                FovMotionData.UpdateType = CameraUpdate.Interpolation;
                break;
            case CameraUpdate.Tween:
                if (!Mathf.IsEqualApprox(_previousFov, finalFov) && !_fovTween.IsRunning())
                    MotionTween(ref _fovTween, "fov", finalFov, FovMotionData, true);
                break;
            case CameraUpdate.Interpolation:
                Fov = MotionInterpolation(Fov, finalFov, FovMotionData.LerpWeight, delta);
                break;
        }
    }

    public Vector3 MotionInterpolation(Vector3 motionStart, Vector3 motionTarget, float motionWeight, float delta)
    {
        if (motionStart.IsEqualApprox(motionTarget))
            return motionStart;
                
        Vector3 nextValue = motionStart.Lerp(motionTarget, motionWeight * delta);
        if (nextValue.IsEqualApprox(motionTarget))
            return motionTarget;

        return nextValue;
    }
    
    public float MotionInterpolation(float motionStart, float motionTarget, float motionWeight, float delta)
    {
        if (Mathf.IsEqualApprox(motionStart, motionTarget))
            return motionStart;
                
        float nextValue = Mathf.Lerp(motionStart, motionTarget, motionWeight * delta);
        if (Mathf.IsEqualApprox(nextValue, motionTarget))
            return motionTarget;

        return nextValue;
    }

    public void MotionTween(ref Tween tween, string propertyName, Variant motionTarget, CameraMotionData motionData, bool force = false)
    {
        if (tween == null || (tween != null && tween.IsRunning() && force))
        {
            tween?.Kill();
            tween = CreateTween();
        }
        
        tween.TweenProperty(this, property: propertyName, motionTarget, motionData.TweenDuration)
            .SetTrans(motionData.TweenTrans)
            .SetEase(motionData.TweenEase);
    }
}
