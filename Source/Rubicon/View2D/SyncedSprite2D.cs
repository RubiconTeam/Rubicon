namespace Rubicon.View2D;

// TODO: Gotta find a way to make this better later... -Binpuki
#if TOOLS
[Tool] 
#endif
public partial class SyncedSprite2D : AnimatedSprite2D
{
	[Export] public bool Playing
	{
		get
		{
			if (AnimationPlayer != null)
				return AnimationPlayer.IsPlaying();

			return IsPlaying();
		}
		set
		{
			if (AnimationPlayer != null)
				return;
			
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
	
	[ExportGroup("Sync With"), Export] public AnimationPlayer AnimationPlayer;
	
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!Playing)
			return;
		
		if (AnimationPlayer != null)
			SyncWithAnimationPlayer();
	}

	public void SyncWithAnimationPlayer()
	{
		if (IsPlaying())
			Stop();
		
		double time = AnimationPlayer.CurrentAnimationPosition;
		double fps = SpriteFrames.GetAnimationSpeed(Animation);

		Frame = (int)Math.Floor(time * fps);
		FrameProgress = (float)(time % (1 / fps) * fps);
	}
}
