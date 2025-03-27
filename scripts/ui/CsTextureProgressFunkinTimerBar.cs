using Rubicon.API;
using Rubicon.Core;
using Rubicon.Core.Data;

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
        switch (Bar.FillMode)
        {
            case (int)TextureProgressBar.FillModeEnum.BilinearLeftAndRight:
            case (int)TextureProgressBar.FillModeEnum.BilinearTopAndBottom:
            case (int)TextureProgressBar.FillModeEnum.ClockwiseAndCounterClockwise:
            {
                switch (Direction)
                {
                    case BarDirection.LeftToRight:
                        Bar.Ratio = ProgressRatio;
                        break;
                    case BarDirection.RightToLeft:
                        Bar.Ratio = 1f - ProgressRatio;
                        break;
                }
                break;
            }
            default:
            {
                Bar.Ratio = ProgressRatio;
                break;
            }
        }
        
        float time = Mathf.Clamp(Length - Conductor.RawTime, 0f, Length);
        TimeLabel.Text = $"({TimeSpan.FromSeconds(time):mm\\:ss})";
    }

    protected override void ChangeLeftColor(Color leftColor)
    {
        switch (Direction)
        {
            case BarDirection.LeftToRight:
            {
                switch (Bar.FillMode)
                {
                    case (int)TextureProgressBar.FillModeEnum.BilinearLeftAndRight:
                    case (int)TextureProgressBar.FillModeEnum.BilinearTopAndBottom:
                    case (int)TextureProgressBar.FillModeEnum.ClockwiseAndCounterClockwise:
                        break;
                    default:
                    {
                        Bar.TintUnder = leftColor;
                        break;
                    }
                }
                break;
            }
            case BarDirection.RightToLeft:
            {
                switch (Bar.FillMode)
                {
                    case (int)TextureProgressBar.FillModeEnum.BilinearLeftAndRight:
                    case (int)TextureProgressBar.FillModeEnum.BilinearTopAndBottom:
                    case (int)TextureProgressBar.FillModeEnum.ClockwiseAndCounterClockwise:
                        break;
                    default:
                    {
                        Bar.TintProgress = leftColor;
                        break;
                    }
                }
                break;
            }
        }
    }

    protected override void ChangeRightColor(Color rightColor)
    {
        switch (Direction)
        {
            case BarDirection.LeftToRight:
            {
                switch (Bar.FillMode)
                {
                    case (int)TextureProgressBar.FillModeEnum.BilinearLeftAndRight:
                    case (int)TextureProgressBar.FillModeEnum.BilinearTopAndBottom:
                    case (int)TextureProgressBar.FillModeEnum.ClockwiseAndCounterClockwise:
                        break;
                    default:
                    {
                        Bar.TintProgress = rightColor;
                        break;
                    }
                }
                break;
            }
            case BarDirection.RightToLeft:
            {
                switch (Bar.FillMode)
                {
                    case (int)TextureProgressBar.FillModeEnum.BilinearLeftAndRight:
                    case (int)TextureProgressBar.FillModeEnum.BilinearTopAndBottom:
                    case (int)TextureProgressBar.FillModeEnum.ClockwiseAndCounterClockwise:
                        break;
                    default:
                    {
                        Bar.TintUnder = rightColor;
                        break;
                    }
                }
                break;
            }
        }
    }

    protected override void ChangeDirection(BarDirection direction)
    {
        switch (direction)
        {
            case BarDirection.LeftToRight:
            {
                switch (Bar.FillMode)
                {
                    case (int)TextureProgressBar.FillModeEnum.LeftToRight: 
                    case (int)TextureProgressBar.FillModeEnum.RightToLeft:
                    {
                        Bar.FillMode = (int)TextureProgressBar.FillModeEnum.LeftToRight;
                        break;
                    }
                    case (int)TextureProgressBar.FillModeEnum.Clockwise:
                    case (int)TextureProgressBar.FillModeEnum.CounterClockwise:
                    {
                        Bar.FillMode = (int)TextureProgressBar.FillModeEnum.Clockwise;
                        break;
                    }
                }
                break;
            }
            case BarDirection.RightToLeft:
            {
                switch (Bar.FillMode)
                {
                    case (int)TextureProgressBar.FillModeEnum.LeftToRight: 
                    case (int)TextureProgressBar.FillModeEnum.RightToLeft:
                    {
                        Bar.FillMode = (int)TextureProgressBar.FillModeEnum.RightToLeft;
                        break;
                    }
                    case (int)TextureProgressBar.FillModeEnum.Clockwise:
                    case (int)TextureProgressBar.FillModeEnum.CounterClockwise:
                    {
                        Bar.FillMode = (int)TextureProgressBar.FillModeEnum.CounterClockwise;
                        break;
                    }
                }

                Bar.FillMode = (int)TextureProgressBar.FillModeEnum.RightToLeft;
                break;
            }
        }
        
        ChangeLeftColor(LeftColor);
        ChangeRightColor(RightColor);
    }
}