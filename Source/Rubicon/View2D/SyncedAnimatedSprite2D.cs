namespace Rubicon.View2D;

// TODO: Gotta find a way to make this better later... -Binpuki
#if TOOLS
[Tool] 
#endif
public partial class SyncedAnimatedSprite2D : AnimatedSprite2D
{
	[Export] public bool Playing
	{
		get => IsPlaying();
		set
		{
			if (value)
			{
				Play(Animation);
				return;
			}
			
			int curFrame = Frame;
			Stop();
			Frame = curFrame;
		}
	}
	
	
}
