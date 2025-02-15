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
    /// Position update type.
    /// See <see cref="CameraUpdate"/> for update types.
    /// </summary>
    [ExportGroup("Update Settings"), Export] public CameraUpdate PositionUpdateType = CameraUpdate.Interpolation;
    
    /// <summary>
    /// Rotation update type.
    /// See <see cref="CameraUpdate"/> for update types.
    /// </summary>
    [Export] public CameraUpdate RotationUpdateType = CameraUpdate.Interpolation;
    
    /// <summary>
    /// Zoom update type.
    /// See <see cref="CameraUpdate"/> for update types.
    /// </summary>
    [Export] public CameraUpdate ZoomUpdateType = CameraUpdate.Interpolation;

    /// <summary>
    /// Weight value when calculating position lerp.
    /// Only affects <see cref="CameraUpdate.Interpolation"/> update type.
    /// </summary>
    [ExportSubgroup("Interpolation"), Export] public float PositionLerpWeight = 2.4f;
    
    /// <summary>
    /// Weight value when calculating rotation lerp.
    /// Only affects <see cref="CameraUpdate.Interpolation"/> update type.
    /// </summary>
    [Export] public float RotationLerpWeight = 2.4f;
    
    /// <summary>
    /// Weight value when calculating zoom lerp.
    /// Only affects <see cref="CameraUpdate.Interpolation"/> update type.
    /// </summary>
    [Export] public float ZoomLerpWeight = 3.125f;

    /// <summary>
    /// Duration of the position tweens.
    /// Only affects <see cref="CameraUpdate.Tween"/> update type.
    /// </summary>
    [ExportSubgroup("Tweening"), Export] public float PositionTweenDuration = 1f;
    
    /// <summary>
    /// Duration of the rotation tweens.
    /// Only affects <see cref="CameraUpdate.Tween"/> update type.
    /// </summary>
    [Export] public float RotationTweenDuration = 1f;
    
    /// <summary>
    /// Duration of the zoom tweens.
    /// Only affects <see cref="CameraUpdate.Tween"/> update type.
    /// </summary>
    [Export] public float ZoomTweenDuration = 1f;
    
    private Vector2 _previousPosition = Vector2.Zero;
    private float _previousRotation = 0;
    private Vector2 _previousZoom = Vector2.Zero;
    
    private Tween _posTween;
    private Tween _rotTween;
    private Tween _zoomTween;

    public override void _Process(double delta)
    {
        PositionSmoothingEnabled = PositionUpdateType == CameraUpdate.Smoothing;
        
        float deltaF = (float)delta;
        UpdatePosition(deltaF);
        UpdateZoom(deltaF);
    }

    /// <summary>
    /// Updates the camera's position depending on <see cref="PositionUpdateType"/>.
    /// </summary>
    public virtual void UpdatePosition(float delta)
    {
        Vector2 finalPosition = TargetPosition + OffsetPosition;
        switch (PositionUpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Smoothing:
                GlobalPosition = finalPosition;
                break;
            case CameraUpdate.Tween:
                if (_previousPosition != finalPosition && !_posTween.IsRunning())
                    TweenPosition(finalPosition, PositionTweenDuration, true);
                break;
            case CameraUpdate.Interpolation:
                Vector2 pos = GlobalPosition;
                if (pos.IsEqualApprox(finalPosition))
                    return;
                
                Vector2 nextPos = pos.Lerp(finalPosition, PositionLerpWeight * delta);
                if (nextPos.IsEqualApprox(finalPosition))
                {
                    GlobalPosition = finalPosition;
                    return;
                }

                GlobalPosition = nextPos;
                break;
        }
    }
    
    /// <summary>
    /// Updates the camera's rotation depending on <see cref="RotationUpdateType"/>.
    /// </summary>
    public virtual void UpdateRotation(float delta)
    {
        float finalRotation = TargetRotation + OffsetRotation;
        switch (PositionUpdateType)
        {
            case CameraUpdate.Instant:
            case CameraUpdate.Smoothing:
                GlobalRotation = finalRotation;
                break;
            case CameraUpdate.Tween:
                if (!Mathf.IsEqualApprox(_previousRotation, finalRotation) && !_rotTween.IsRunning())
                    TweenRotation(finalRotation, RotationTweenDuration, true);
                break;
            case CameraUpdate.Interpolation:
                float rot = GlobalRotation;
                if (Mathf.IsEqualApprox(rot, finalRotation))
                    return;
                
                float nextPos = Mathf.Lerp(rot, finalRotation, RotationLerpWeight * delta);
                if (Mathf.IsEqualApprox(nextPos, finalRotation))
                {
                    GlobalRotation = finalRotation;
                    return;
                }

                GlobalRotation = nextPos;
                break;
        }
    }
    
    /// <summary>
    /// Updates the camera's zoom depending on <see cref="ZoomUpdateType"/>.
    /// </summary>
    public virtual void UpdateZoom(float delta)
    {
        //Vector2 combinedZoom = TargetZoom + OffsetZoom;
        Vector2 finalZoom = TargetZoom + OffsetZoom;
        switch (ZoomUpdateType)
        {
            case CameraUpdate.Instant:
                Zoom = finalZoom;
                break;
            case CameraUpdate.Interpolation:
                Vector2 zoom = Zoom;
                if (zoom.IsEqualApprox(finalZoom))
                    return;
                
                Vector2 nextZoom = zoom.Lerp(finalZoom, ZoomLerpWeight * delta);
                if (nextZoom.IsEqualApprox(finalZoom))
                {
                    Zoom = finalZoom;
                    return;
                }

                Zoom = nextZoom;
                break;
            case CameraUpdate.Tween:
                if (_previousZoom != finalZoom && !_zoomTween.IsRunning())
                    TweenZoom(finalZoom, ZoomTweenDuration, true);
                break;
        }
    }
    
    /// <summary>
    /// Tween the position of this camera to the final position.
    /// </summary>
    /// <param name="position">The final position</param>
    /// <param name="duration">How long the tween will last</param>
    /// <param name="force">Setting this to true will immediately kill the previous, otherwise this will queue this tween so that it is ran when the previous one is finished.</param>
    public void TweenPosition(Vector2 position, double duration, bool force = false)
    {
        if (_posTween == null || (_posTween != null && _posTween.IsRunning() && force))
        {
            _posTween?.Kill();
            _posTween = CreateTween();
        }
        
        _posTween.TweenProperty(this, property: "global_position", position, duration);
    }

    /// <summary>
    /// Tween the rotation of this camera to the final rotation.
    /// </summary>
    /// <param name="rotation">The final rotation</param>
    /// <param name="duration">How long the tween will last</param>
    /// <param name="force">Setting this to true will immediately kill the previous, otherwise this will queue this tween so that it is ran when the previous one is finished.</param>
    public void TweenRotation(float rotation, double duration, bool force = false)
    {
        if (_rotTween == null || (_rotTween != null && _rotTween.IsRunning() && force))
        {
            _rotTween?.Kill();
            _rotTween = CreateTween();
        }
        
        _rotTween.TweenProperty(this, property: "global_rotation", rotation, duration);
    }

    
    /// <summary>
    /// Tween the zoom of this camera to the zoom specified.
    /// </summary>
    /// <param name="zoom">The zoom specified</param>
    /// <param name="duration">How long the tween will last</param>
    /// <param name="force">Setting this to true will immediately kill the previous, otherwise this will queue this tween so that it is ran when the previous one is finished.</param>
    public void TweenZoom(Vector2 zoom, double duration, bool force = false)
    {
        if (_zoomTween == null || (_zoomTween != null && _zoomTween.IsRunning() && force))
        {
            _zoomTween?.Kill();
            _zoomTween = CreateTween();
        }
        
        _zoomTween.TweenProperty(this, property: "zoom", zoom, duration);
    }
}
