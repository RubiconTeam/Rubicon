using Rubicon.API;

namespace Rubicon.Extras.UI;

[GlobalClass] public partial class RubiconTimerBar : CsTimerBar
{
    [Export] public Label Text;

    [Export] public CanvasItem FillCircle;

    public override void UpdateTimer(float currentTime, float length)
    {
        float time = Mathf.Clamp(length - currentTime, 0f, length);
        Text.Text = $"({TimeSpan.FromSeconds(time):mm\\:ss})";
        
        (FillCircle.Material as ShaderMaterial).SetShaderParameter("value", Mathf.Clamp(currentTime / length, 0f, 1f));
    }
}