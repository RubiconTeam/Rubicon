using Rubicon.API;
using Rubicon.Core;

namespace Rubicon.Extras.UI;

/// <summary>
/// A Funkin' timer bar that utilizes a <see cref="TextureProgressBar"/>
/// </summary>
[GlobalClass] public partial class CsTextureProgressFunkinTimerBar : CsFunkinTimerBar
{
    /// <summary>
    /// The <see cref="TextureProgressBar"/> associated with this health bar.
    /// </summary>
    [Export] public TextureProgressBar Bar;
    
    /// <summary>
    /// The visual text for the time remaining.
    /// </summary>
    [Export] public Label TimeLabel;

    public override void OptionsUpdated() { }

    protected override void UpdateBar()
    {
        Bar.Ratio = ProgressRatio;
        
        float time = Mathf.Clamp(Length - Conductor.RawTime, 0f, Length);
        TimeLabel.Text = $"({TimeSpan.FromSeconds(time):mm\\:ss})";
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