using Rubicon.Data;

namespace Rubicon.View2D;

[GlobalClass]
public partial class RubiconCamera2D : Camera2D
{
    [Export] public Vector2 TargetPosition = Vector2.Zero;
    [Export] public Vector2 OffsetPosition = Vector2.Zero;
    [Export] public float TargetRotation = 0f;
    [Export] public float OffsetRotation = 0f;
    [Export] public Vector2 TargetZoom = Vector2.One;
    [Export] public Vector2 OffsetZoom = Vector2.Zero;

    [ExportGroup("Settings"), Export] public CameraUpdate PositionUpdateType = CameraUpdate.Interpolation;
    [Export] public CameraUpdate RotationUpdateType = CameraUpdate.Interpolation;
    [Export] public CameraUpdate ZoomUpdateType = CameraUpdate.Interpolation;

    [ExportSubgroup("Interpolation"), Export] public float PositionLerpWeight = 2.4f;
    [Export] public float RotationLerpWeight = 2.4f;
    [Export] public float ZoomLerpWeight = 57f;

    [ExportSubgroup("Tweening"), Export] public float PositionTweenDuration = 1f;
    [Export] public float RotationTweenDuration = 1f;
    [Export] public float ZoomTweenDuration = 1f;
    
    /* OG by lego, just referencing it here
    [Export] public bool ZoomSmoothingEnabled = true;
    [Export] public Vector2 StaticZoom;
    [Export] public float ZoomSmoothingSpeed = 3f;
    public Tween ZoomTween;*/
    
    private Vector2 _previousPosition = Vector2.Zero;
    private Vector2 _previousZoom = Vector2.Zero;
    
    private Tween _posTween;
    private Tween _zoomTween;

    public override void _Process(double delta)
    {
        PositionSmoothingEnabled = PositionUpdateType == CameraUpdate.Smoothing;
        
        float deltaF = (float)delta;
        UpdatePosition(deltaF);
        UpdateZoom(deltaF);
    }

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
