namespace Rubicon.View2D;

// TODO: Gotta find a way to make this better later... -Binpuki
#if TOOLS
[Tool] 
#endif
public partial class PlayerSprite2D : AnimatedSprite2D
{
	[Export] public bool Playing
	{
		get => Playing;
		set
		{
			if (value) Play(Animation);
			else
			{
				int curFrame = Frame;
				Stop();
				Frame = curFrame;
			}
		}
	}
}
