using Rubicon.Core.Data;
using Rubicon.Core.UI;
using Rubicon.Data.Settings;
using Rubicon.Game;
using Rubicon.Rulesets;

namespace Rubicon.Extras.UI;

/// <summary>
/// A Judgment class that mimics the animations from Dance Dance Revolution.
/// </summary>
public partial class DdrJudgment : Control, IJudgment, IJudgmentMaterial
{
    /// <summary>
    /// Textures to fetch from when displaying judgments.
    /// </summary>
    [Export] public SpriteFrames Atlas;

    /// <summary>
    /// How much to scale the judgment graphics by.
    /// </summary>
    [Export] public Vector2 GraphicScale = Vector2.One;
    
    /// <summary>
    /// The default opacity each time the judgment activates.
    /// </summary>
    [Export] public float Opacity = 0.5f;

    /// <inheritdoc/>
    public Material PerfectMaterial { get; set; } // dokibird glasses

    /// <inheritdoc/>
    public Material GreatMaterial { get; set; }

    /// <inheritdoc/>
    public Material GoodMaterial { get; set; }

    /// <inheritdoc/>
    public Material OkayMaterial { get; set; }
    
    /// <inheritdoc/>
    public Material BadMaterial { get; set; }

    /// <inheritdoc/>
    public Material MissMaterial { get; set; }
    
    private Control _judgmentControl;
    private AnimatedSprite2D _judgmentGraphic;
    private Tween _judgeTween;
    private Vector2 _offset = Vector2.Zero;
    
    /// <inheritdoc/>
    public void Play(HitType type, Vector2? offset)
    {
        if (RubiconGame.Instance != null && RubiconGame.Instance.PlayField != null)
        {
            PlayField playField = RubiconGame.Instance.PlayField;
            BarLine barLine = playField.BarLines[playField.TargetIndex];
            _offset = offset ?? Vector2.Zero;

            Vector2 pos = barLine.GlobalPosition + (_offset * (UserSettings.Gameplay.DownScroll ? -1f : 1f));
            Play(type, barLine.AnchorLeft, barLine.AnchorTop, barLine.AnchorRight, barLine.AnchorBottom, pos);
            return;
        }
        
        Play(type, 0, 0f, 0f, 0f, offset);
    }
    
    /// <inheritdoc/>
    public void Play(HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        if (_judgmentGraphic == null)
        {
            _judgmentControl = new Control();
            _judgmentControl.Name = "Judgment Container";
            _judgmentGraphic = new AnimatedSprite2D();
            _judgmentGraphic.Name = "Graphic";
            
            _judgmentControl.AddChild(_judgmentGraphic);
            AddChild(_judgmentControl);
        }
        _judgeTween?.Kill();

        _judgmentControl.AnchorLeft = anchorLeft;
        _judgmentControl.AnchorTop = anchorTop;
        _judgmentControl.AnchorRight = anchorRight;
        _judgmentControl.AnchorBottom = anchorBottom;
        _judgmentGraphic.SpriteFrames = Atlas;
        _judgmentGraphic.Animation = type.ToString();
        _judgmentGraphic.Frame = 0;
        _judgmentGraphic.Play();
        _judgmentGraphic.Material = this.GetHitMaterial(type);
        _judgmentControl.Scale = GraphicScale * 1.1f;
        _judgmentControl.Position = pos ?? Vector2.Zero;
        _judgmentControl.Modulate = new Color(_judgmentControl.Modulate.R, _judgmentControl.Modulate.G,
            _judgmentControl.Modulate.B, Opacity);

        _judgeTween = _judgmentControl.CreateTween();
        _judgeTween.TweenProperty(_judgmentControl, "scale", GraphicScale, 0.1d);
        _judgeTween.TweenProperty(_judgmentControl, "modulate", Colors.Transparent, 0.5d).SetDelay(0.4d);
        _judgeTween.Play();
    }
    
    public override void _Process(double delta)
    {
        if (RubiconGame.Instance == null || RubiconGame.Instance.PlayField == null || _judgmentControl == null || _judgmentGraphic == null)
            return;
        
        PlayField playField = RubiconGame.Instance.PlayField;
        BarLine barLine = playField.BarLines[playField.TargetIndex];
        _judgmentControl.Position = barLine.GlobalPosition + (_offset * (UserSettings.Gameplay.DownScroll ? -1f : 1f));
    }
}