namespace Rubicon.View2D;

// TODO: Gotta find a way to make this better later... -Binpuki
#if TOOLS
[Tool] 
#endif
public partial class SyncedSprite2D : AnimatedSprite2D
{
	[Export] public bool Playing { get => GetPlaying(); set => SetPlaying(value); }

	[Export] public new int Frame { get => base.Frame; set => SetFrame(value); }

	[Export] public new float FrameProgress { get => base.FrameProgress; set => SetFrameProgress(value); }

	[ExportGroup("Sync With"), Export] public AnimationPlayer AnimationPlayer;
	
	private double _time = 0.0;
	private double _lastPlayerPosition = 0.0;
	private bool _playing = false;
	
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (AnimationPlayer == null)
			return;
		
		if (IsPlaying())
			Stop();

		double currentPos = AnimationPlayer.CurrentAnimationPosition;
		SyncWithAnimationPlayer(_playing ? currentPos - _lastPlayerPosition : 0.0);
		_lastPlayerPosition = currentPos;
	}

	public void SyncWithAnimationPlayer(double delta)
	{
		_time += delta;
		if (_time < 0.0)
			_time = 0.0;
			
		double fps = SpriteFrames.GetAnimationSpeed(Animation);

		base.Frame = (int)Math.Floor(_time * fps);
		base.FrameProgress = (float)(_time % (1 / fps) * fps);
	}

	private bool GetPlaying()
	{
		if (AnimationPlayer == null)
			return IsPlaying();

		return _playing;
	}
	
	private void SetPlaying(bool value)
	{
		_playing = value;
		if (_playing)
		{
			if (AnimationPlayer != null)
				return;
			
			Play(Animation);
			return;
		}

		Pause();
	}

	private new void SetFrame(int value)
	{
		FrameProgress = 0;

		double frameRate = SpriteFrames.GetAnimationSpeed(Animation);
		double fps = 1 / frameRate;
		_time = value * fps + FrameProgress * fps;
		if (_time < 0.0)
			_time = 0.0;
		
		base.Frame = (int)Math.Floor(_time * frameRate);
	}

	private new void SetFrameProgress(float value)
	{
		base.SetFrameProgress(value);
			
		double frameRate = SpriteFrames.GetAnimationSpeed(Animation);
		double fps = 1 / frameRate;
		_time = Frame * fps + value * fps;
		if (_time < 0.0)
			_time = 0.0;
	}
}
