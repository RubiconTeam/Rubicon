using Rubicon.API;

namespace Rubicon.Extras.UI;

/// <summary>
/// A Funkin' health bar that utilizes a <see cref="TextureProgressBar"/>
/// </summary>
[GlobalClass] public partial class CsTextureProgressFunkinHealthBar : CsFunkinHealthBar
{
    /// <summary>
    /// The <see cref="TextureProgressBar"/> associated with this health bar.
    /// </summary>
    [Export] public TextureProgressBar Bar;

    public override void OptionsUpdated() { }

    protected override void UpdateBar()
    {
        Bar.Ratio = ProgressRatio;
    }

    protected override void ChangeLeftColor(Color leftColor)
    {
        Bar.TintProgress = leftColor;
    }

    protected override void ChangeRightColor(Color rightColor)
    {
        Bar.TintUnder = rightColor;
    }
}