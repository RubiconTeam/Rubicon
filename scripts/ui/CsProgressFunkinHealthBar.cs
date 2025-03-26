using Rubicon.API;

namespace Rubicon.Extras.UI;

/// <summary>
/// A Funkin' health bar that utilizes a <see cref="ProgressBar"/>
/// </summary>
[GlobalClass] public partial class CsProgressFunkinHealthBar : CsFunkinHealthBar
{
    /// <summary>
    /// The <see cref="ProgressBar"/> associated with this health bar.
    /// </summary>
    [Export] public ProgressBar Bar;

    private StyleBox _leftFill;
    private StyleBox _rightFill;

    public override void _Ready()
    {
        base._Ready();

        _leftFill = Bar.GetThemeStylebox("theme_override_styles/fill");
        _rightFill = Bar.GetThemeStylebox("theme_override_styles/background");
    }

    public override void OptionsUpdated() { }

    protected override void UpdateBar()
    {
        Bar.Ratio = ProgressRatio;
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
}