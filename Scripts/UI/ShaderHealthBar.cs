using Rubicon.API;
using Rubicon.Core.Data;
using Rubicon.Core.Rulesets;
using Rubicon.Game;

namespace Rubicon.Extras.UI;

/// <summary>
/// A health bar specifically to be worked with the shader in res://Resources/Shaders/BarShader.gdshader
/// </summary>
public partial class ShaderHealthBar : CsHealthBar
{
    [Export] public CanvasItem Bar;

    [Export] public Control IconContainer;
    
    private ShaderMaterial _material;

    public override void _Ready()
    {
        base._Ready();
        
        _material = Bar.Material as ShaderMaterial;
    }

    protected override void UpdateBar(float progress, BarDirection direction)
    {
        float value = direction != BarDirection.LeftToRight ? 1f - progress : progress;
        (Bar.Material as ShaderMaterial).SetShaderParameter("value", value);

        IconContainer.AnchorLeft = IconContainer.AnchorRight = value;
    }

    protected override void ChangeLeftColor(Color leftColor)
    {
        _material.SetShaderParameter("black", leftColor);
    }

    protected override void ChangeRightColor(Color rightColor)
    {
        _material.SetShaderParameter("white", rightColor);
    }
}