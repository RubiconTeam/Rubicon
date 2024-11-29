using System.Linq;
using Godot.Collections;
using Rubicon.Core.Data;
using Rubicon.Core.UI;
using Rubicon.Data.Settings;
using Rubicon.Game;
using Rubicon.Rulesets;

namespace Rubicon.Extras.UI;

/// <summary>
/// A ComboDisplay class that mimics the animations from Dance Dance Revolution.
/// </summary>
public partial class DdrComboDisplay : Control, IComboDisplay, IJudgmentMaterial
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
    private Array<Control> _comboGraphics = new();
    private Array<Tween> _comboTweens = new();
    private Vector2 _offset = Vector2.Zero;

    /// <inheritdoc/>
    public void Show(long combo, HitType type, Vector2? offset)
    {
	    if (RubiconGame.Instance != null && RubiconGame.Instance.PlayField != null)
	    {
		    PlayField playField = RubiconGame.Instance.PlayField;
		    BarLine barLine = playField.BarLines[playField.TargetIndex];
		    _offset = offset ?? Vector2.Zero;

		    Vector2 pos = barLine.GlobalPosition + (_offset * (UserSettings.Gameplay.DownScroll ? -1f : 1f));
		    Show(combo, type, barLine.AnchorLeft, barLine.AnchorTop, barLine.AnchorRight, barLine.AnchorBottom, pos);
		    return;
	    }
        
	    Show(combo, type, 0f, 0f, 0f, 0f, offset);
    }
    
    /// <inheritdoc/>
    public void Show(long combo, HitType type, float anchorLeft, float anchorTop, float anchorRight, float anchorBottom, Vector2? pos)
    {
	    if (type > _lastRating)
		    _lastRating = type;
	    
        for (int i = 0; i < _comboTweens.Count; i++)
	        _comboTweens[i].Kill();
        _comboTweens.Clear();

        string comboString = combo.ToString();
		string[] splitDigits = new String[comboString.Length];
		for (int i = 0; i < splitDigits.Length; i++)
			splitDigits[i] = comboString.ToCharArray()[i].ToString();

		int childCount = GetChildCount();
		for (int i = 0; i < childCount; i++)
			_comboGraphics[i].Modulate = Colors.Transparent;
		
		if (splitDigits.Length <= childCount)
		{
			for (int i = 0; i < splitDigits.Length; i++)
			{
				Control comboSpr = _comboGraphics[i];
				AnimatedSprite2D comboGraphic = comboSpr.GetChild<AnimatedSprite2D>(0);

				comboGraphic.SpriteFrames = Atlas;
				comboGraphic.Animation = splitDigits[i];
				comboGraphic.Frame = 0;
				comboGraphic.Play();

				comboSpr.Modulate = new Color(1, 1, 1, 0.5f);
				comboSpr.Scale = Vector2.One;
			}
		}
		else if (splitDigits.Length > childCount)
		{
			for (int i = 0; i < childCount; i++)
			{
				Control comboSpr = _comboGraphics[i];
				AnimatedSprite2D comboGraphic = comboSpr.GetChild<AnimatedSprite2D>(0);

				comboGraphic.SpriteFrames = Atlas;
				comboGraphic.Animation = splitDigits[i];
				comboGraphic.Frame = 0;
				comboGraphic.Play();

				comboSpr.Modulate = new Color(1, 1, 1, 0.5f);
				comboSpr.Scale = Vector2.One;
			}

			for (int i = childCount; i < splitDigits.Length; i++)
			{
				Control comboSpr = new TextureRect();
				AnimatedSprite2D comboGraphic = new AnimatedSprite2D();

				comboGraphic.SpriteFrames = Atlas;
				comboGraphic.Animation = splitDigits[i];
				comboGraphic.Frame = 0;
				comboGraphic.Play();
				comboSpr.AddChild(comboGraphic);
					
				comboSpr.Modulate = new Color(1, 1, 1, 0.5f);
				comboSpr.Scale = Vector2.One;
				_comboGraphics.Add(comboSpr);
				AddChild(comboSpr);
			}
		}

		float generalSize = Spacing;
		Vector2 position = pos ?? Vector2.Zero;
		for (int i = 0; i < splitDigits.Length; i++)
		{
			Control comboSpr = _comboGraphics[i];
			AnimatedSprite2D comboGraphic = comboSpr.GetChild<AnimatedSprite2D>(0);

			comboGraphic.Material = this.GetHitMaterial(_lastRating);
			comboSpr.AnchorLeft = anchorLeft;
			comboSpr.AnchorTop = anchorTop;
			comboSpr.AnchorRight = anchorRight;
			comboSpr.AnchorBottom = anchorBottom;
			comboSpr.Position = new Vector2(i * generalSize - ((splitDigits.Length - 1) * generalSize / 2), 0) -
			                    comboSpr.PivotOffset + position;
		        
			comboSpr.Modulate = new Color(comboSpr.Modulate.R, comboSpr.Modulate.G, comboSpr.Modulate.B, 0.5f);
			comboSpr.Scale = new Vector2(1.2f, 1.2f) * GraphicScale;

			Tween comboTwn = comboSpr.CreateTween();
			comboTwn.TweenProperty(comboSpr, "scale", GraphicScale, 0.2);
			comboTwn.TweenProperty(comboSpr, "modulate", new Color(1f, 1f, 1f, 0f), 1).SetDelay(0.8);
			_comboTweens.Add(comboTwn);
		}

		if (combo == 0)
			_lastRating = HitType.Perfect;
    }
    
    public override void _Process(double delta)
    {
	    if (RubiconGame.Instance == null || RubiconGame.Instance.PlayField == null)
		    return;
        
	    PlayField playField = RubiconGame.Instance.PlayField;
	    BarLine barLine = playField.BarLines[playField.TargetIndex];
	    Vector2 startPos = barLine.GlobalPosition + (_offset * (UserSettings.Gameplay.DownScroll ? -1f : 1f));
	    
	    int comboCount = _comboGraphics.Count(x => x.Modulate.A != 0);
	    for (int i = 0; i < comboCount; i++)
			_comboGraphics[i].Position = startPos + new Vector2(i * Spacing - ((comboCount - 1) * Spacing / 2), 0) - _comboGraphics[i].PivotOffset;
    }
}