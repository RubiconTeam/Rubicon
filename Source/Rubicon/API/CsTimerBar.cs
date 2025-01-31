using Rubicon.Core;
using Rubicon.Game;

namespace Rubicon.API;

[GlobalClass] public abstract partial class CsTimerBar : Control
{
    private float _length = 0f;

    public override void _Ready()
    {
        base._Ready();
        
        RubiconGame game = RubiconGame.Instance;
        if (game is null)
            return;

        _length = (float)game.Instrumental.Stream.GetLength();
    }

    public override void _Process(double delta)
    {
        RubiconGame game = RubiconGame.Instance;
        if (game is null)
            return;
        
        UpdateTimer(Conductor.Time, _length);
    }

    public abstract void UpdateTimer(float currentTime, float length);
}