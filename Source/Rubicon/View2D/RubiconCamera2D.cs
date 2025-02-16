using Rubicon.Data;

namespace Rubicon.View2D;

/// <summary>
/// <see cref="Camera2D"/> utility that manages smooth positioning, tweening and zooming. 
/// </summary>

[GlobalClass]
public partial class RubiconCamera2D : Camera2D
{
    /// <summary>
    /// The final position to smoothly move to.
    /// </summary>
    [Export] public Vector2 TargetPosition = Vector2.Zero;
    
    /// <summary>
    /// Offset added to <see cref="TargetPosition"/> when calculating a new position.
    /// Similar to <see cref="Camera2D.Offset"/>, except it also gets smoothed.
    /// </summary>
    [Export] public Vector2 OffsetPosition = Vector2.Zero;
    
    /// <summary>
    /// The final rotation to smoothly move to.
    /// </summary>
    [Export] public float TargetRotation = 0f;
    
    /// <summary>
    /// Offset added to <see cref="TargetRotation"/> when calculating a new rotation.
    /// </summary>
    [Export] public float OffsetRotation = 0f;
    
    /// <summary>
    /// The final zoom to smoothly move to.
    /// </summary>
    [Export] public Vector2 TargetZoom = Vector2.One;
    
    /// <summary>
    /// Offset added to <see cref="TargetZoom"/> when calculating a new zoom value.
    /// </summary>
    [Export] public Vector2 OffsetZoom = Vector2.Zero;
    
    /// <summary>
    /// Values used when updating the camera's position.
    /// </summary>
    [ExportGroup("Motion Settings"), Export] public CameraMotionData PositionMotionData = new();
    
    /// <summary>
    /// Values used when updating the camera's rotation.
    /// </summary>
    [Export] public CameraMotionData RotationMotionData = new();
    
    /// <summary>
    /// Values used when updating the camera's zoom.
    /// </summary>
    [Export] public CameraMotionData ZoomMotionData = new();
    
    private Vector2 _previousPosition = Vector2.Zero;
    private float _previousRotation = 0;
    private Vector2 _previousZoom = Vector2.Zero;

    public override void _Process(double delta)
    {
        if (PositionMotionData == null && RotationMotionData == null && ZoomMotionData == null)
            return;
        
        PositionSmoothingEnabled = PositionMotionData.UpdateType == CameraUpdate.Smoothing;
        
        float deltaF = (float)delta;
        UpdatePosition(deltaF);
        UpdateRotation(deltaF);
        UpdateZoom(deltaF);
    }
    
    /// <summary>
    /// Updates the camera's position depending on the position's <see cref="CameraMotionData"/>.
    /// </summary>
    public virtual void UpdatePosition(float delta)
    {
        Vector2 finalPosition = TargetPosition + OffsetPosition;
        switch (PositionMotionData.UpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Smoothing:
                GlobalPosition = finalPosition;
                break;
            case CameraUpdate.Tween:
                _previousPosition = GlobalPosition;
                if (!_previousPosition.IsEqualApprox(finalPosition))
                    MotionTween("global_position", finalPosition, PositionMotionData, false);
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
        float finalRotation = TargetRotation + OffsetRotation;
        switch (RotationMotionData.UpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Smoothing:
                RotationMotionData.UpdateType = CameraUpdate.Interpolation;
                break;
            case CameraUpdate.Tween:
                _previousRotation = GlobalRotation;
                if (!Mathf.IsEqualApprox(_previousRotation, finalRotation))
                    MotionTween("global_rotation", finalRotation, RotationMotionData, false);
                break;
            case CameraUpdate.Interpolation:
                GlobalRotation = MotionInterpolation(GlobalRotation, finalRotation, RotationMotionData.LerpWeight, delta);
                break;
        }
    }
    
    /// <summary>
    /// Updates the camera's zoom depending on the zoom's <see cref="CameraMotionData"/>.
    /// </summary>
    public virtual void UpdateZoom(float delta)
    {
        Vector2 finalZoom = TargetZoom + OffsetZoom;
        switch (ZoomMotionData.UpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Smoothing:
                ZoomMotionData.UpdateType = CameraUpdate.Interpolation;
                break;
            case CameraUpdate.Tween:
                _previousZoom = Zoom;
                if (!_previousZoom.IsEqualApprox(finalZoom))
                    MotionTween("zoom", finalZoom, ZoomMotionData, false);
                break;
            case CameraUpdate.Interpolation:
                Zoom = MotionInterpolation(Zoom, finalZoom, ZoomMotionData.LerpWeight, delta);
                break;
        }
    }

    public Vector2 MotionInterpolation(Vector2 motionStart, Vector2 motionTarget, float motionWeight, float delta)
    {
        if (motionStart.IsEqualApprox(motionTarget))
            return motionStart;
                
        Vector2 nextValue = motionStart.Lerp(motionTarget, motionWeight * delta);
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

    // still gotta figure out whats wrong with this
    // at least it doesnt make new tweens every second now
    // - legole0
    public void MotionTween(string propertyName, Variant motionTarget, CameraMotionData motionData, bool force = false)
    {
        if (motionData.RunningTween && !force)
            return;
        
        Tween tween = motionData.MotionTween;
        if (tween == null || (tween != null && tween.IsRunning() && force))
        {
            tween?.Kill();
            tween = CreateTween().SetParallel();
            motionData.RunningTween = true;
            GD.Print($"tween {propertyName} created");
        }
        
        tween.TweenProperty(this, property: propertyName, motionTarget, motionData.TweenDuration)
            .SetTrans(motionData.TweenTrans)
            .SetEase(motionData.TweenEase);
        tween.Finished += () => motionData.RunningTween = false;
    }
}
