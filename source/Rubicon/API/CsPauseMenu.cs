using PukiTools.GodotSharp;
using Rubicon.Game;

namespace Rubicon.API;

/// <summary>
/// A template for a pause menu in Rubicon.
/// </summary>
[GlobalClass] public abstract partial class CsPauseMenu : CsMenu
{
    /// <summary>
    /// The action in the input map that pauses the game.
    /// </summary>
    [Export] public string PauseAction = "game_pause";
    
    /// <summary>
    /// Triggers after the pause action is invoked.
    /// </summary>
    [Signal] public delegate void PauseOpenedEventHandler();
    
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        
        if (!@event.IsActionPressed(PauseAction) || !RubiconGame.Active)
            return;

        if (RubiconGame.Paused || RubiconGame.PlayField.HasFailed())
            return;
        
        
        RubiconGame.Pause();
        OpenPause();
        EmitSignalPauseOpened();
    }

    /// <summary>
    /// Invokes when the pause action is invoked.
    /// </summary>
    public abstract void OpenPause();
}