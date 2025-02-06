namespace Rubicon.View2D;

// TODO: Gotta find a way to make this better later... -Binpuki
#if TOOLS
[Tool] 
#endif
public partial class SyncedSprite2D : AnimatedSprite2D
{
	[Export] public bool Sync { get => GetSync(); set => SetSync(value); }

	[Export] public int FrameOffset = 0;

	[ExportGroup("Sync With"), Export] public AnimationPlayer AnimationPlayer;
	
	private double _time = 0.0;
	private double _lastPlayerPosition = 0.0;
	private bool _syncing = false;
	
	public override void _Process(double delta)
	{
		base._Process(delta);
		
		bool isOnAnimation = AnimationPlayer != null && (!string.IsNullOrEmpty(AnimationPlayer.AssignedAnimation) || !string.IsNullOrEmpty(AnimationPlayer.CurrentAnimation) || AnimationPlayer.IsPlaying());
		if (!isOnAnimation)
			return;
		
		if (IsPlaying())
			Stop();

		double currentPos = AnimationPlayer.CurrentAnimationPosition;
		if (Mathf.IsEqualApprox(currentPos, _lastPlayerPosition))
			return;
		
		SyncWithAnimationPlayer(_syncing ? currentPos - _lastPlayerPosition : 0.0);
		_lastPlayerPosition = currentPos;
	}

	public void SyncWithAnimationPlayer(double delta)
	{
		_time += delta;
		if (_time < 0.0)
			_time = 0.0;
			
		double fps = SpriteFrames.GetAnimationSpeed(Animation);

		Frame = FrameOffset + (int)Math.Floor(_time * fps);
		FrameProgress = (float)(_time % (1 / fps) * fps);
	}

	private bool GetSync()
	{
		return _syncing;
	}
	
	private void SetSync(bool value)
	{
		_syncing = value;
		if (_syncing)
		{
			if (AnimationPlayer != null)
				return;
			
			Play(Animation);
			return;
		}

		Pause();
	}
}
