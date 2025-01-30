using Rubicon.Core.Rulesets;
using Rubicon.Game;

namespace Rubicon.Extras.UI;

/// <summary>
/// A health bar specifically to be worked with the shader in res://Resources/Shaders/BarShader.gdshader
/// </summary>
public partial class ShaderHealthBar : Control
{
    [Export] public CanvasItem Bar;
    
    private ShaderMaterial _material;
    private int _previousHealth = 0;

    public override void _Ready()
    {
        base._Ready();
        
        _material = Bar.Material as ShaderMaterial;
        
        RubiconGame game = RubiconGame.Instance;
        if (game == null)
            return;
        
        
    }

    public override void _Process(double delta)
    {
        if (_previousHealth == PlayField.Instance.Health)
            return;
        
        _material.SetShaderParameter("value", (float)PlayField.Instance.Health / PlayField.Instance.MaxHealth);
    }
}