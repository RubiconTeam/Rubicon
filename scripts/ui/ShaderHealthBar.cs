using Rubicon.API;
using Rubicon.Core.Data;

namespace Rubicon.Extras.UI;

/// <summary>
/// A health bar specifically to be worked with the shader in res://resources/shaders/BarShader.gdshader
/// </summary>
public partial class ShaderHealthBar : CsFunkinHealthBar
{
    [Export] public CanvasItem Bar;
    
    private ShaderMaterial _material;

    public override void Initialize()
    {
        _material = Bar.Material as ShaderMaterial;
        
        base.Initialize();
    }

    protected override void UpdateBar(float progress, BarDirection direction)
    {
        float value = direction != BarDirection.LeftToRight ? 1f - progress : progress;
        (Bar.Material as ShaderMaterial).SetShaderParameter("value", value);

        IconContainer.AnchorLeft = IconContainer.AnchorRight = value;
    }

    protected override void ChangeLeftColor(Color leftColor)
    {
        (Bar.Material as ShaderMaterial).SetShaderParameter("black", leftColor);
    }

    protected override void ChangeRightColor(Color rightColor)
    {
        (Bar.Material as ShaderMaterial).SetShaderParameter("white", rightColor);
    }
}