using Rubicon.API;
using Rubicon.Core;
using Rubicon.Core.Data;

namespace Rubicon.Extras.UI;

/// <summary>
/// A Funkin' health bar that utilizes a <see cref="ProgressBar"/>
/// </summary>
[GlobalClass] public partial class CsProgressFunkinTimerBar : CsFunkinTimerBar
{
    /// <summary>
    /// The <see cref="ProgressBar"/> associated with this health bar.
    /// </summary>
    [Export] public ProgressBar Bar;

    /// <summary>
    /// The visual text for the time remaining.
    /// </summary>
    [Export] public Label TimeLabel;

    private StyleBox _fillStyle;
    private StyleBox _underStyle;
    
    private StyleBox _leftFill;
    private StyleBox _rightFill;

    public override void _Ready()
    {
        _fillStyle = Bar.GetThemeStylebox("fill").Duplicate() as StyleBox;
        _underStyle = Bar.GetThemeStylebox("background").Duplicate() as StyleBox;
        
        Bar.AddThemeStyleboxOverride("fill", _fillStyle);
        Bar.AddThemeStyleboxOverride("background", _underStyle);
        
        base._Ready();
    }

    public override void OptionsUpdated() { }

    protected override void UpdateBar()
    {
        Bar.Ratio = ProgressRatio;
        
        float time = Mathf.Clamp(Length - Conductor.RawTime, 0f, Length);
        TimeLabel.Text = $"({TimeSpan.FromSeconds(time):mm\\:ss})";
    }

    protected override void ChangeLeftColor(Color leftColor)
    {
        if (_leftFill is StyleBoxFlat flat)
            flat.BgColor = leftColor;
        else if (_leftFill is StyleBoxLine line)
            line.Color = leftColor;
        else if (_leftFill is StyleBoxTexture texture)
            texture.ModulateColor = leftColor;
    }

    protected override void ChangeRightColor(Color rightColor)
    {
        if (_rightFill is StyleBoxFlat flat)
            flat.BgColor = rightColor;
        else if (_rightFill is StyleBoxLine line)
            line.Color = rightColor;
        else if (_rightFill is StyleBoxTexture texture)
            texture.ModulateColor = rightColor;
    }
    
    protected override void ChangeDirection(BarDirection direction)
    {
        switch (direction)
        {
            case BarDirection.LeftToRight:
                Bar.FillMode = (int)ProgressBar.FillModeEnum.BeginToEnd;
                _leftFill = _fillStyle;
                _rightFill = _underStyle;
                break;
            case BarDirection.RightToLeft:
                Bar.FillMode = (int)ProgressBar.FillModeEnum.EndToBegin;
                _leftFill = _underStyle;
                _rightFill = _fillStyle;
                break;
        }
        
        ChangeLeftColor(LeftColor);
        ChangeRightColor(RightColor);
    }
}