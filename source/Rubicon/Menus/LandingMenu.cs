using PukiTools.GodotSharp.Audio;

namespace Rubicon.Menus;

[GlobalClass] public partial class LandingMenu : Menu
{
    [Export] public Control Background;

    [Export] public float StartingBackgroundY = 0;

    [Export] public float Spacing = 180f;
    
    [Export] public float BackgroundLerpWeight = 3.6f;
    
    [ExportGroup("Audio"), Export] public AudioStream MoveSound;

    [Export] public AudioStream ConfirmSound;

    private int _currentlyFocusedIndex = -1;

    public override void _Ready()
    {
        base._Ready();

        _currentlyFocusedIndex = Array.IndexOf(Focusable, InitialFocus);
    }

    public override void UpdateSelection(Control focused)
    {
        _currentlyFocusedIndex = Array.IndexOf(Focusable, focused);
        AudioManager.GetGroup("SoundEffects").Play(MoveSound, true);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Background == null)
            return;
        
        Vector2 pos = Background.Position;
        pos.Y = Mathf.Lerp(pos.Y, StartingBackgroundY + Spacing * _currentlyFocusedIndex, BackgroundLerpWeight * (float)delta);
        Background.Position = pos;
    }

    public void Confirm()
    {
        AudioManager.GetGroup("SoundEffects").Play(ConfirmSound, true);
    }
}