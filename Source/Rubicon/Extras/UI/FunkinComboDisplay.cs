using System.Linq;
using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Data;
using Rubicon.Core.UI;

namespace Rubicon.Extras.UI;

/// <summary>
/// A <see cref="ComboDisplay"/> class that mimics the animation style of Friday Night Funkin'.
/// </summary>
public partial class FunkinComboDisplay : Control, IComboDisplay, IJudgmentMaterial
{
    /// <summary>
    /// The textures to display. Should go from 0 to 9.
    /// </summary>
    [Export] public SpriteFrames Atlas;
    
    /// <summary>
    /// The spacing of the textures.
    /// </summary>
    [Export] public float Spacing = 100f;

    /// <summary>
    /// The scale of the textures.
    /// </summary>
    [Export] public Vector2 GraphicScale = Vector2.One;
    
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

    private HitType _lastRating = HitType.Perfect;
    private bool _wasZero = false;
    private Array<Control> _comboGraphics = new();
    private Dictionary<Control, Vector2> _comboVelocities = new();
    private Dictionary<Control, int> _comboAccelerations = new();

    /// <inheritdoc/>
    public void Show(long combo, HitType type, Vector2? offset)
    {
        Show(combo, type, 0.5f, 0.5f, 0.5f, 0.5f, new Vector2((Size.X * 0.507f) - 97.5f, Size.Y * 0.48f) + offset);
    }
    
    /// <inheritdoc/>
    public void Show(long combo, HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
        if (combo == 0 && _wasZero)
            return;

        if (type > _lastRating)
            _lastRating = type;
        
        string comboString = combo.ToString("D3");
        string[] splitDigits = new string[comboString.Length];
        for (int i = 0; i < splitDigits.Length; i++)
            splitDigits[i] = comboString.ToCharArray()[i].ToString();
        
        float generalSize = Spacing;
        Control[] currentGraphics = new Control[splitDigits.Length];
        for (int i = 0; i < splitDigits.Length; i++)
        {
            Control comboSpr = _comboGraphics.FirstOrDefault(x => x.Modulate.A == 0 && !currentGraphics.Contains(x));
            if (comboSpr == null)
            {
                comboSpr = new TextureRect();
                comboSpr.Name = $"Instance {_comboGraphics.Count}";
                AnimatedSprite2D comboGraphic = new AnimatedSprite2D();
                comboGraphic.Name = "Graphic";
                comboGraphic.Centered = false;
                
                comboSpr.AddChild(comboGraphic);
                _comboGraphics.Add(comboSpr);
                AddChild(comboSpr);
            }

            AnimatedSprite2D graphic = comboSpr.GetChild<AnimatedSprite2D>(0);
            graphic.SpriteFrames = Atlas;
            graphic.Animation = splitDigits[i];
            graphic.Frame = 0;
            graphic.Play();
            graphic.Material = this.GetHitMaterial(_lastRating);
            
            comboSpr.MoveToFront();
            comboSpr.Scale = GraphicScale;
            comboSpr.Position = (pos ?? Vector2.Zero) + new Vector2(i * generalSize, 0);
            comboSpr.Modulate = new Color(comboSpr.Modulate.R, comboSpr.Modulate.G, comboSpr.Modulate.B);

            currentGraphics[i] = comboSpr;
            _comboVelocities[comboSpr] = new Vector2(GD.RandRange(1, 15), GD.RandRange(-160, -140));
            _comboAccelerations[comboSpr] = GD.RandRange(300, 450);

            Tween fadeTween = comboSpr.CreateTween();
            fadeTween.TweenProperty(comboSpr, "modulate", Colors.Transparent, 0.2d)
                .SetDelay(60d / Conductor.Bpm * 2d);
            fadeTween.Play();
        }

        _wasZero = combo == 0;
        if (_wasZero)
            _lastRating = HitType.Perfect;
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Theoretically I could use Rigidbody2Ds but I think running the positioning in Process makes it look better :)
        for (int i = 0; i < _comboGraphics.Count; i++)
        {
            if (_comboGraphics[i].Modulate.A == 0)
                continue;

            Control comboSpr = _comboGraphics[i];
            Vector2 velocity = _comboVelocities[comboSpr];
            int acceleration = _comboAccelerations[comboSpr];

            comboSpr.Position += velocity * (float)delta;
            _comboVelocities[comboSpr] += new Vector2(0f, acceleration * (float)delta);
        }
    }
}