using Rubicon.API;
using Rubicon.Core;
using Rubicon.Core.Data;

namespace Rubicon.Extras.UI;

/// <summary>
/// A Funkin' timer bar that utilizes a bar with a shader applied to it.
/// </summary>
public partial class CsShaderFunkinTimerBar : CsFunkinTimerBar
{
    /// <summary>
    /// The CanvasItem that has the shader applied to it.
    /// </summary>
    [Export] public CanvasItem Bar;
    
    /// <summary>
    /// The visual text for the time remaining.
    /// </summary>
    [Export] public Label TimeLabel;

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
        float time = Mathf.Clamp(Length - Conductor.RawTime, 0f, Length);
        TimeLabel.Text = $"({TimeSpan.FromSeconds(time):mm\\:ss})";
        
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
    
    protected override void ChangeDirection(BarDirection direction) { }
}