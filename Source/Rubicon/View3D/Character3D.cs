using Rubicon.Core;
using Rubicon.Data;

namespace Rubicon.View3D;
[GlobalClass] public partial class Character3D : Node3D
{
    /// <summary>
	/// The icon data for this character.
	/// </summary>
	[Export] public CharacterIconData Icon = new();
	
    /// <summary>
    /// Determines which axis is the character facing at,
    /// and if it should be flipped or not.
    /// </summary>
    [Export] public bool FrontFacing;

    /// <summary>
    /// Gets added to the start of EVERY animation that plays after this is set. Overridden by  <see cref="CharacterAnimation.CustomSuffix"/> when using <see cref="PlayAnim"/>.
    /// </summary>
    [Export] public string GlobalPrefix;

    /// <summary>
    /// Gets added to the end of EVERY animation that plays after this is set. Overridden by  <see cref="CharacterAnimation.CustomSuffix"/> when using <see cref="PlayAnim"/>.
    /// </summary>
    [Export] public string GlobalSuffix;
    
    /// <summary>
    /// A string array containing the sequence of idle/dance animations to be played.
    /// </summary>
    [Export] public string[] DanceList = ["idle"];

    /// <summary>
    /// The time of type to go by with <see cref="DanceTime"/>.
    /// </summary>
    [Export] public TimeValue DanceType
    {
	    get => _timeType;
	    set
	    {
		    _timeType = value;

		    if (Bumper != null)
			    Bumper.Type = _timeType;
	    }
    }
    
    /// <summary>
    /// How often to dance.
    /// </summary>
    [Export] public float DanceTime
    {
	    get
	    {
		    if (Bumper != null)
			    return Bumper.Value;

		    return _danceValue;
	    }
	    set
	    {
		    _danceValue = value;
		    
		    if (Bumper != null)
			    Bumper.Value = _danceValue;
	    }
    }

    /// <summary>
    /// Determines how holding is handled animation-wise.
    /// </summary>
    [Export] public CharacterHold HoldType = CharacterHold.Freeze;

    /// <summary>
    /// How long the singing animation should last in steps before idling back.
    /// </summary>
    [Export] public float SingDuration = 4;

    /// <summary>
    /// A timer that determines if the animation should be finished or not.
    /// </summary>
    [Export] public float SingTimer;

    /// <summary>
    /// Turning this to false will prevent this character from dancing, unless manually called.
    /// </summary>
    [Export] public bool CanDance = true;

    /// <summary>
    /// The reference visual node.
    /// </summary>
    [ExportGroup("References"), Export] public Node3D VisualObject;

    /// <summary>
    /// The animation controller for this character.
    /// </summary>
    [Export] public AnimationPlayer AnimationPlayer;

    /// <summary>
    /// A <see cref="Node3D"/> from which the camera takes its position. Recommended to use <see cref="Camera3D"/>
    /// </summary>
    [Export] public Node3D CameraPoint;
    
    /// <summary>
    /// The index for which dance animation in the <see cref="DanceList"/> should play.
    /// </summary>
    [ExportGroup("Internals"), Export] public int DanceIndex;
    
    /// <summary>
    /// Flips left and right animations depending on if <see cref="LeftFacing"/> != <see cref="SpawnPoint3D"/>'s <see cref="SpawnPoint3D.LeftFacing"/>
    /// </summary>
    [Export] public bool FlipAnimations;
    
    /// <summary>
    /// The animation that is currently being played.
    /// </summary>
    [Export] public CharacterAnimation CurrentAnim;

    /// <summary>
    /// The animation that was last played.
    /// </summary>
    [Export] public CharacterAnimation LastAnim;
    
    /// <summary>
    /// The node that helps with dancing to the beat.
    /// </summary>
    [Export] public Bumper Bumper;

    /// <summary>
    /// Whether this character is currently singing or not.
    /// </summary>
    [Export] public bool Singing;
    
    /// <summary>
    /// Marks whether this character is holding a note.
    /// </summary>
    [Export] public bool Holding;

    /// <summary>
    /// Whether this character should freeze after holding (and not go to idle)
    /// </summary>
    [Export] public bool FreezeSinging;

    private TimeValue _timeType = TimeValue.Beat;
    private float _danceValue = 1f / 2f;
    
    private int _lastStep = -int.MaxValue;

    public override void _Ready()
    {
        base._Ready();
        
        // replace Camera3D with Node3D
        // (its probably fine for performance but you never know)
        if (CameraPoint is Camera3D cameraPoint)
        {
	        Node3D newCameraPoint = new Node3D();
	        newCameraPoint.Position = cameraPoint.Position;
	        newCameraPoint.Rotation = cameraPoint.Rotation;
	        
	        CameraPoint.Free();
	        CameraPoint = newCameraPoint;
        }

        Bumper = new Bumper();
        Bumper.Name = "Bumper";
        AddChild(Bumper);

        Bumper.Type = _timeType;
        Bumper.Value = _danceValue;
        Bumper.Bumped += TryDance;

        AnimationPlayer.AnimationFinished += AnimationFinished;
        Dance();
    }

    public override void _Process(double delta)
    {
	    base._Process(delta);
	    
	    // Hacky hold method, should work for now
	    int curStep = Mathf.FloorToInt(Conductor.CurrentStep);
	    if (Holding)
			HandleHoldAnimations();

	    SingTimer += (float)delta;
	    _lastStep = curStep;
    }

    /// <summary>
    /// Plays a dance animation for this character.
    /// </summary>
    /// <param name="force">Force this animation to play</param>
    public void Dance(bool force = false)
    {
	    if (!force && AnimationPlayer.IsPlaying() && AnimationPlayer.CurrentAnimationPosition < AnimationPlayer.CurrentAnimationLength) 
		    return;

	    Singing = false;
	    PlayAnimation(new CharacterAnimation { Name = DanceList[DanceIndex], Force = true, StartTime = 0});
	    DanceIndex = (DanceIndex + 1) % DanceList.Length;
    }

    /// <summary>
    /// Plays a sing animation for this character.
    /// </summary>
    /// <param name="direction">Marks the direction to sing at</param>
    /// <param name="holding">Marks this as a holding animation</param>
    /// <param name="miss">Marks this as a miss animation</param>
    public void Sing(string direction, bool holding = false, bool miss = false)
    {
	    string animName = $"sing{direction.ToUpper()}" + (miss ? "miss" : "");
	    CharacterAnimation singAnim = new CharacterAnimation
	    {
			Name = animName,
			Force = true,
			StartTime = 0f
	    };
	    
	    SingWithCustomAnimation(singAnim, holding);
    }

    public void SingWithCustomAnimation(CharacterAnimation anim, bool holding = false)
    {
	    SingTimer = 0f;
	    Singing = true;
	    Holding = holding;
	    
	    PlayAnimation(anim);
    }

    /// <summary>
    /// Plays an animation for this character.
    /// </summary>
    /// <param name="anim">Animation data</param>
    public void PlayAnimation(CharacterAnimation anim)
    {
	    if (!anim.Force && AnimationPlayer.IsPlaying() && AnimationPlayer.CurrentAnimationPosition >= AnimationPlayer.CurrentAnimationLength)
		    return;

	    string originalName = anim.Name; 
	    if (FlipAnimations) 
		    anim.Name = anim.Name.Contains("LEFT") ? anim.Name.Replace("LEFT", "RIGHT") : anim.Name.Replace("RIGHT", "LEFT");

	    string prefix = !string.IsNullOrEmpty(anim.CustomPrefix) ? anim.CustomPrefix : GlobalPrefix;
	    string suffix = !string.IsNullOrEmpty(anim.CustomSuffix) ? anim.CustomSuffix : GlobalSuffix;
	    
	    anim.Name = $"{prefix}{anim.Name}{suffix}";
	    if (!AnimationPlayer.HasAnimation(anim.Name))
	    {
		    GD.PushWarning($"There is no animation in AnimationPlayer called {anim.Name}. (Original Animation: {originalName})");
		    return;
	    }
	    
	    LastAnim = CurrentAnim;
	    CurrentAnim = anim;
	    
	    if (LastAnim == null || (LastAnim != null && anim.Name != LastAnim.Name)) 
		    AnimationPlayer.Play(anim.Name);
	    
	    AnimationPlayer.Seek(anim.StartTime, true);
	    AnimationPlayer.Play(anim.Name);
    }

    public void PlayAnimationWithName(string animName, bool force = false, float startTime = 0f)
    {
	    CharacterAnimation anim = new CharacterAnimation
	    {
			Name = animName,
			Force = force,
			StartTime = startTime
	    };
	    
	    PlayAnimation(anim);
    }

    public Vector3 GetCameraPosition()
    {
	    return CameraPoint.Position;
    }

    protected virtual void HandleHoldAnimations()
    {
	    switch (HoldType)
	    {
		    case CharacterHold.None:
			    SingTimer = 0;
			    break;
		    case CharacterHold.StepJitter:
			    int curStep = Mathf.FloorToInt(Conductor.CurrentStep);
			    if (_lastStep != curStep)
			    {
				    AnimationPlayer.Seek(0f);
				    SingTimer = 0;
			    }
			    
			    _lastStep = curStep;
			    break;
		    case CharacterHold.Jitter:
			    if (AnimationPlayer.CurrentAnimationPosition < 0.125)
				    break;
			    
			    AnimationPlayer.Seek(0f);
			    SingTimer = 0;
			    break;
		    case CharacterHold.Freeze:
			    AnimationPlayer.Seek(0f);
			    SingTimer = 0;
			    break;
	    }	
    }
    
    private void TryDance()
    {
	    if (CanDance && CurrentAnim != null && (Singing && !FreezeSinging && SingTimer >= Conductor.StepValue * 0.001f * SingDuration || !CurrentAnim.Name.StartsWith("sing")))
		    Dance(true);
    }

    private void AnimationFinished(StringName anim) 
    {
        if (CurrentAnim.PostAnimation != null)
            PlayAnimation(CurrentAnim.PostAnimation);
    }
}
