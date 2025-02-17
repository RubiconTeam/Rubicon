using Rubicon.Screens;

namespace Rubicon.API;

/// <summary>
/// A template for loading screens in C#.
/// </summary>
[GlobalClass] public partial class CsLoadingScreen : Control
{
    /// <summary>
    /// The transition into the loading screen.
    /// </summary>
    [Export] public StringName OpeningAnimation;
    
    /// <summary>
    /// The transition out of the loading screen.
    /// </summary>
    [Export] public StringName ClosingAnimation;
    
    /// <summary>
    /// The animation player reference.
    /// </summary>
    [Export] public AnimationPlayer AnimationPlayer;

    /// <summary>
    /// Readies the loading screen!
    /// </summary>
    public override void _Ready()
    {
        base._Ready();

        AnimationPlayer.AnimationFinished += AnimationFinished;
        AnimationPlayer.Play(OpeningAnimation);
        AnimationPlayer.Seek(0, true);

        ScreenManager.Completed += LoadCompleted;
    }

    private void AnimationFinished(StringName anim)
    {
        if (anim == OpeningAnimation)
            ScreenManager.StartLoading();
        
        if (anim == ClosingAnimation)
            QueueFree();
    }
    
    private void LoadCompleted()
    {
        ScreenManager.Completed -= LoadCompleted;
        
        AnimationPlayer.Play(ClosingAnimation);
        AnimationPlayer.Seek(0, true);
    }
}