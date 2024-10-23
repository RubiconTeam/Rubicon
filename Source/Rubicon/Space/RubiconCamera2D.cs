
namespace Rubicon.Space;
public partial class RubiconCamera2D : Camera2D
{
    [Export] public bool ZoomSmoothingEnabled = true;
    [Export] public Vector2 StaticZoom;
    [Export] public float ZoomSmoothingSpeed = 3f;
    public Tween ZoomTween;

    public override void _Ready()
    {
        if (StaticZoom == Vector2.Zero)
            StaticZoom = Zoom;
    }

    public void LerpZoom(float LerpSpeed)
    {
        Zoom = new Vector2(
            Mathf.Lerp(Zoom.X, StaticZoom.X, ZoomSmoothingSpeed),
            Mathf.Lerp(Zoom.Y, StaticZoom.Y, ZoomSmoothingSpeed)
            );
    }

    public void TweenZoom(Vector2 NewZoom, double Duration)
    {
        bool lastSmoothing = ZoomSmoothingEnabled;
        ZoomSmoothingEnabled = false;
        if(ZoomTween != null)
            ZoomTween.Stop();

        ZoomTween = CreateTween();
        ZoomTween.TweenProperty(this, "zoom", NewZoom, Duration);
    }
}
