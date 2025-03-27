using System.Collections.Generic;
using System.Linq;
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
    /// Determines if the character is facing north (Z+ axis),
    /// and if it should be flipped or not.
    /// </summary>
    [Export] public bool FacingZAxis;

    /// <summary>
    /// Gets added to the start of EVERY animation that plays after this is set. Overridden by  <see cref="SpecialAnimation.CustomSuffix"/> when using <see cref="PlayAnim"/>.
    /// </summary>
    [Export] public string GlobalPrefix;

    /// <summary>
    /// Gets added to the end of EVERY animation that plays after this is set. Overridden by  <see cref="SpecialAnimation.CustomSuffix"/> when using <see cref="PlayAnim"/>.
    /// </summary>
    [Export] public string GlobalSuffix;
    
    /// <summary>
    /// If toggled off, the character will wait for the current dance animation to finish before dancing again. 
    /// </summary>
    [Export] public bool ForceDancing = true;
    
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

		    if (BeatSyncer != null)
			    BeatSyncer.Type = _timeType;
	    }
    }
    
    /// <summary>
    /// How often to dance.
    /// </summary>
    [Export] public float DanceTime
    {
	    get
	    {
		    if (BeatSyncer != null)
			    return BeatSyncer.Value;

		    return _danceValue;
	    }
	    set
	    {
		    _danceValue = value;
		    
		    if (BeatSyncer != null)
			    BeatSyncer.Value = _danceValue;
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
    [Export] private Node3D _cameraPoint;
    
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
    [Export] public SpecialAnimation CurrentSpecialParameters = null;
    
    /// <summary>
    /// The node that helps with dancing to the beat.
    /// </summary>
    [Export] public BeatSyncer BeatSyncer;

    /// <summary>
    /// Whether this character is currently singing or not.
    /// </summary>
    [Export] public bool Singing;
    
    /// <summary>
    /// Marks whether this character is holding a note.
    /// </summary>
    [Export] public bool Holding;
    
    /// <summary>
    /// Marks whether this character has missed.
    /// </summary>
    [Export] public bool Missed = false;
    
    /// <summary>
    /// Turning this to false will prevent this character from dancing, unless manually called.
    /// </summary>
    [Export] public bool FreezeDance = false;

    /// <summary>
    /// Whether this character should freeze after holding (and not go to idle)
    /// </summary>
    [Export] public bool FreezeSinging;

    public Node3D CameraPoint;
    
    private TimeValue _timeType = TimeValue.Beat;
    private float _danceValue = 1f / 2f;
    
    private int _lastStep = -int.MaxValue;
    private Dictionary<string, bool> _directionsHolding = new();
    private string _lastDirection = String.Empty;

    public override void _Ready()
    {
        base._Ready();
        
        // replace Camera3D with Node3D
        // (its probably fine for performance but you never know)
        /*if (_cameraPoint is Camera3D cameraPoint)
        {
	        CameraPoint = new Node3D();
	        CameraPoint.GlobalPosition = cameraPoint.GlobalPosition;
	        CameraPoint.GlobalRotationDegrees = cameraPoint.GlobalRotationDegrees;
	        AddChild(CameraPoint);
	        
	        _cameraPoint.QueueFree();
        }*/

        BeatSyncer = new BeatSyncer();
        BeatSyncer.Name = "Bumper";
        AddChild(BeatSyncer);

        BeatSyncer.Type = _timeType;
        BeatSyncer.Value = _danceValue;
        BeatSyncer.Bumped += TryDance;

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
    /// <param name="customPrefix">An optional prefix to put in before the dance animation. Will be <see cref="GlobalPrefix"/> by default.</param>
    /// <param name="customSuffix">An optional suffix to put in after the dance animation. Will be <see cref="GlobalSuffix"/> by default.</param>
    public void Dance(string customPrefix = null, string customSuffix = null)
    {
	    string prefix = customPrefix ?? GlobalPrefix;
	    string suffix = customSuffix ?? GlobalSuffix;

	    Singing = false;
	    AnimationPlayer.Play(prefix + DanceList[DanceIndex] + suffix);
	    AnimationPlayer.Seek(0f, true);
	    
	    DanceIndex = (DanceIndex + 1) % DanceList.Length;
    }

    /// <summary>
    /// Plays a sing animation for this character.
    /// </summary>
    /// <param name="direction">Marks the direction to sing at</param>
    /// <param name="holding">Marks this as a holding animation</param>
    /// <param name="miss">Marks this as a miss animation</param>
    /// <param name="customPrefix">An optional prefix to put in before the sing animation. Will be <see cref="GlobalPrefix"/> by default.</param>
    /// <param name="customSuffix">An optional suffix to put in after the sing animation. Will be <see cref="GlobalSuffix"/> by default.</param>
    public void Sing(string direction, bool holding = false, bool miss = false, string customPrefix = null, string customSuffix = null)
    {
	    if (CurrentSpecialParameters != null && CurrentSpecialParameters.OverrideSing)
		    return;

	    direction = direction.ToUpper();
	    bool wasHolding = _directionsHolding.ContainsKey(direction) && _directionsHolding[direction];
	    _directionsHolding[direction] = holding;
	    bool shouldBeHolding = _directionsHolding.ContainsValue(true);
	    
	    SingTimer = 0f;
	    Singing = true;
	    Holding = shouldBeHolding;
	    Missed = miss && !shouldBeHolding;

	    if (FlipAnimations)
		    direction = direction == "LEFT" ? "RIGHT" : direction == "RIGHT" ? "LEFT" : direction;

	    bool isHoldEnding = wasHolding && !holding;
	    if (isHoldEnding)
	    {
		    switch (HoldType)
		    {
			    case CharacterHold.None:
				    if (!Missed)
					    return;
				    
				    break;
			    default:
				    direction = _lastDirection;
				    break;
		    }
	    }
	    
	    string animName = $"sing{direction.ToUpper()}" + (Missed ? "miss" : "");
	    
	    string prefix = customPrefix ?? GlobalPrefix;
	    string suffix = customSuffix ?? GlobalSuffix;
	    
	    AnimationPlayer.Play(prefix + animName + suffix);
	    AnimationPlayer.Seek(0f, true);
	    
	    _lastDirection = direction;
    }
    
    public void PlaySpecialAnimation(SpecialAnimation anim)
    {
	    CurrentSpecialParameters = anim;
	    
	    AnimationPlayer.Play(anim.Name);
	    AnimationPlayer.Seek(anim.StartTime, true);
    }

    public Vector3 GetCameraPosition()
    {
	    Vector3 position = _cameraPoint.GlobalPosition;
	    if(CameraPoint != null)
		    position = CameraPoint.GlobalPosition;
	    
	    return position;
    }

    public Vector3 GetCameraRotation()
    {
	    Vector3 rotation = _cameraPoint.GlobalRotation;
	    if(CameraPoint != null)
		    rotation = CameraPoint.GlobalRotation;
	    
	    return rotation;
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
			    double normalizedTime =
				    AnimationPlayer.CurrentAnimationPosition / AnimationPlayer.CurrentAnimationLength;
			    if (normalizedTime < 0.125)
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
	    bool isCurrentDanceDone = !AnimationPlayer.IsPlaying() || AnimationPlayer.IsPlaying() &&
		    DanceList.Any(x => x.Contains(AnimationPlayer.CurrentAnimation)) &&
		    AnimationPlayer.CurrentAnimationPosition >= AnimationPlayer.CurrentAnimationLength;
	    bool overrideDancing = CurrentSpecialParameters != null && CurrentSpecialParameters.OverrideDance;
	    if ((ForceDancing || !ForceDancing && isCurrentDanceDone) && !FreezeDance && !overrideDancing && (!Singing || (Singing && !FreezeSinging && SingTimer >= Conductor.StepValue * 0.001f * SingDuration)))
		    Dance();
    }

    private void AnimationFinished(StringName anim)
    {
	    CurrentSpecialParameters = null;
    }
}
