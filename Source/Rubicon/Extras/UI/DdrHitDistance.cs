using Rubicon.Core.Data;
using Rubicon.Core.UI;
using Rubicon.Data.Settings;
using Rubicon.Game;
using Rubicon.Rulesets;

namespace Rubicon.Extras.UI;

/// <summary>
/// A HitDistance class that mimics the animations from Dance Dance Revolution.
/// </summary>
public partial class DdrHitDistance : Label, IHitDistance
{
    /// <summary>
    /// The base scale for this hit distance.
    /// </summary>
    [Export] public Vector2 GraphicScale = Vector2.One;
    
    private Tween _labelTween;
    private Vector2 _offset = Vector2.Zero;
    
    /// <inheritdoc/>
    public void Show(double distance, HitType type, Vector2? offset)
    {
        if (RubiconGame.Instance != null && RubiconGame.Instance.PlayField != null)
        {
            PlayField playField = RubiconGame.Instance.PlayField;
            BarLine barLine = playField.BarLines[playField.TargetBarLineIndex];
            _offset = offset ?? Vector2.Zero;

            Vector2 pos = barLine.GlobalPosition + (_offset * (UserSettings.Gameplay.DownScroll ? -1f : 1f));
            Show(distance, type, barLine.AnchorLeft, barLine.AnchorTop, barLine.AnchorRight, barLine.AnchorBottom, pos);
            return;
        }
        
        Show(distance, type, 0f, 0f, 0f, 0f, offset);
    }
    
    /// <inheritdoc/>
    public void Show(double distance, HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        Text = $"{distance:0.00} ms";
        if (Math.Abs(distance) > ProjectSettings.GetSetting("rubicon/judgments/bad_hit_window").AsDouble())
            Text = $"Too {(distance < 0 ? "late!" : "early!")}";
        
        _labelTween?.Kill();

        AnchorLeft = anchorLeft;
        AnchorTop = anchorTop;
        AnchorRight = anchorRight;
        AnchorBottom = anchorBottom;
        PivotOffset = Size / 2f;
        Position = (pos ?? Vector2.Zero) - PivotOffset;
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B);
        Scale = GraphicScale * 1.1f;

        _labelTween = CreateTween();
        _labelTween.TweenProperty(this, "scale", GraphicScale, 0.1d);
        _labelTween.TweenProperty(this, "modulate", Colors.Transparent, 0.5d).SetDelay(1d);
        _labelTween.Play();
    }

    public override void _Process(double delta)
    {
        if (RubiconGame.Instance == null || RubiconGame.Instance.PlayField == null)
            return;
        
        PlayField playField = RubiconGame.Instance.PlayField;
        BarLine barLine = playField.BarLines[playField.TargetBarLineIndex];
        Position = barLine.GlobalPosition + (_offset * (UserSettings.Gameplay.DownScroll ? -1f : 1f)) - PivotOffset;
    }
}