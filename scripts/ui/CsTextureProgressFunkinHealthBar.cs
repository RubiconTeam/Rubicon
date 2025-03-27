using Rubicon.API;
using Rubicon.Core.Data;

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