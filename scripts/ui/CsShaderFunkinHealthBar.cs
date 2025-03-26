using Rubicon.API;

namespace Rubicon.Extras.UI;

/// <summary>
/// A Funkin' health bar that utilizes a bar with a shader applied to it.
/// </summary>
public partial class CsShaderFunkinHealthBar : CsFunkinHealthBar
{
    /// <summary>
    /// The CanvasItem that has the shader applied to it.
    /// </summary>
    [Export] public CanvasItem Bar;

    /// <summary>
    /// Whatever the value property is called in the shader.
    /// </summary>
    [Export] public string ValueProperty = "value";

    /// <summary>
    /// Whatever the left color property is called in the shader.
    /// </summary>
    [Export] public string LeftShaderProperty = "black";
    
    /// <summary>
    /// Whatever the right color property is called in the shader.
    /// </summary>
    [Export] public string RightShaderProperty = "white";

    private ShaderMaterial _material;

    public override void _Ready()
    {
        base._Ready();
        
        _material = Bar.Material as ShaderMaterial;
    }

    public override void OptionsUpdated() { }

    protected override void UpdateBar()
    {
        if (_material == null) return;
        
        _material.SetShaderParameter(ValueProperty, ProgressRatio);
    }

    protected override void ChangeLeftColor(Color leftColor)
    {
        if (_material == null) return;
        
        _material.SetShaderParameter(LeftShaderProperty, leftColor);
    }

    protected override void ChangeRightColor(Color rightColor)
    {
        if (_material == null) return;
        
        _material.SetShaderParameter(RightShaderProperty, rightColor);
    }
}