using Rubicon.Core;
using Rubicon.Data;

namespace Rubicon.View2D;

/// <summary>
/// Character class for <see cref="CanvasItemSpace"/> spaces.
/// </summary>
[GlobalClass] public partial class Character2D : Node2D
{
    /// <summary>
    /// Determines whether the character is facing left or not.
    /// </summary>
    [Export] public bool LeftFacing = false;

    /// <summary>
    /// Flips left and right animations depending on if <see cref="LeftFacing"/> != <see cref="SpawnPoint2D"/>'s <see cref="SpawnPoint2D.LeftFacing"/>
    /// </summary>
    public bool FlipAnimations = false;

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
    /// The index for which dance animation in the <see cref="DanceList"/> should play.
    /// </summary>
    public int DanceIndex = 0;

    /// <summary>
    /// The time of type to go by with <see cref="BounceTime"/>.
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
		    switch (DanceType)
		    {
			    case TimeValue.Measure:
				    return _danceMeasure;
			    case TimeValue.Beat:
				    return ConductorUtility.MeasureToBeats(_danceMeasure);
			    case TimeValue.Step:
				    return ConductorUtility.MeasureToSteps(_danceMeasure);
		    }

		    return 0; // ????
	    }
	    set
	    {
		    switch (DanceType)
		    {
			    case TimeValue.Measure:
				    _danceMeasure = value;
				    break;
			    case TimeValue.Beat:
				    _danceMeasure = ConductorUtility.BeatsToMeasures(value);
				    break;
			    case TimeValue.Step:
				    _danceMeasure = ConductorUtility.StepsToMeasures(value);
				    break;
		    }

		    if (Bumper != null)
			    Bumper.Value = _danceMeasure;
	    }
    }

    /// <summary>
    /// Determines how holding is handled animation-wise.
    /// </summary>
    [Export] public CharacterHold HoldType = CharacterHold.Freeze;

    /// <summary>
    /// The animation that is currently being played.
    /// </summary>
    public CharacterAnimation CurrentAnim = null;

    /// <summary>
    /// The animation that was last played.
    /// </summary>
    public CharacterAnimation LastAnim = null;

    /// <summary>
    /// How long the singing animation should last in steps before idling back.
    /// </summary>
    [Export] public float SingDuration = 4;

    /// <summary>
    /// A timer that determines if the animation should be finished or not.
    /// </summary>
    [Export] public float SingTimer;

    /// <summary>
    /// A timer that determines if the hold animation should be repeated or not.
    /// </summary>
    public float HoldTimer;

    /// <summary>
    /// Marks whether this character is holding a note.
    /// </summary>
    public bool Holding = false;

    /// <summary>
    /// Whether this character should freeze after holding (and not go to idle)
    /// </summary>
    public bool FreezeSinging = false;

    /// <summary>
    /// The node that helps with dancing to the beat.
    /// </summary>
    public Bumper Bumper;

    /// <summary>
    /// <see cref="SpriteFrames"/> used for the health bar. Needs a "neutral" animation, but can optionally have a "lose" or "win" animation as well.
    /// </summary>
    [ExportGroup("Health Bar"), Export] public SpriteFrames Icon;

    /// <summary>
    /// Moves this character's icon in pixels.
    /// </summary>
    [Export] public Vector2 IconOffset = new(0f, 10f);

    /// <summary>
    /// Used as the color representing this character on the health bar.
    /// </summary>
    [Export] public Color HealthColor = new("#A1A1A1");

    /// <summary>
    /// The reference visual node. Usually a <see cref="SyncedSprite2D"/>.
    /// </summary>
    [ExportGroup("References"), Export] public Node2D Sprite;

    /// <summary>
    /// The animation controller for this character.
    /// </summary>
    [Export] public AnimationPlayer AnimationPlayer;

    /// <summary>
    /// A <see cref="Node2D"/> from which the camera takes its position. Recommended to use <see cref="Marker2D"/>
    /// </summary>
    [Export] public Node2D CameraPoint;

    private TimeValue _timeType = TimeValue.Beat;
    private float _danceMeasure = 1f / 2f;
    
    private int _lastStep = -int.MaxValue;

    public override void _Ready()
    {
        base._Ready();

        Bumper = new Bumper();
        Bumper.Name = "Bumper";
        AddChild(Bumper);

        Bumper.Type = TimeValue.Measure;
        Bumper.Value = _danceMeasure;
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

	    PlayAnim(new CharacterAnimation { Name = DanceList[DanceIndex], Force = true, StartTime = 0});
	    
	    DanceIndex++;
	    DanceIndex = (DanceIndex + 1) % DanceList.Length;
    }

    /// <summary>
    /// Plays a sing animation for this character.
    /// </summary>
    /// <param name="direction">Marks the direction to sing at</param>
    /// <param name="holding">Marks this as a holding animation</param>
    /// <param name="miss">Marks this as a miss animation</param>
    /// <param name="customPrefix">Gets added to the start of the anim name</param>
    /// <param name="customSuffix">Gets added to the end of the anim name</param>
    public virtual void Sing(string direction, bool holding = false, bool miss = false, string customPrefix = null, string customSuffix = null)
    {
	    SingTimer = 0f;
	    HoldTimer = 0f;

	    Holding = holding;

	    string animName = $"sing{direction.ToUpper()}" + (miss ? "miss" : "");
	    CharacterAnimation singAnim = new CharacterAnimation
	    {
			Name = animName,
			Force = true,
			StartTime = 0f,
			CustomPrefix = customPrefix,
			CustomSuffix = customSuffix
	    };
	    
	    // Help
	    if (AnimationPlayer.HasAnimation(animName + "-post"))
	    {
		    CharacterAnimation postAnim = new CharacterAnimation
		    {
			    Name = animName + "-post",
			    Force = true,
			    StartTime = 0f,
			    CustomPrefix = customPrefix,
			    CustomSuffix = customSuffix,
		    };

		    singAnim.PostAnimation = postAnim;
	    }
	    
	    PlayAnim(singAnim);
    }

    /// <summary>
    /// Plays an animation for this character.
    /// </summary>
    /// <param name="anim">Animation data</param>
    public void PlayAnim(CharacterAnimation anim)
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

    public Vector2 GetCameraPosition()
    {
	    Node curNode = CameraPoint;
	    Vector2 pos = Vector2.Zero;
	    while (curNode != null)
	    {
		    Node parent = curNode.GetParent();
		    if (curNode is Parallax2D or ParallaxLayer or ParallaxBackground or not CanvasItem)
		    {
			    curNode = parent;
			    continue;
		    }

		    Vector2 posAdd = Vector2.Zero;
		    
		    if (curNode is Control controlNode)
			    posAdd = controlNode.Position.Rotated(controlNode.Rotation);
		    if (curNode is Node2D node2D)
			    posAdd = node2D.Position.Rotated(node2D.Rotation);
		    
		    if (parent is Node2D parent2d and not ParallaxLayer)
			    posAdd *= parent2d.Scale;
		    if (parent is Control controlParent)
			    posAdd *= controlParent.Scale;

		    pos += posAdd;

		    curNode = parent;
	    }

	    return pos;
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
	    if (CurrentAnim != null && (CurrentAnim.Name.StartsWith("sing") && !FreezeSinging && SingTimer >= Conductor.StepValue * 0.001f * SingDuration || !CurrentAnim.Name.StartsWith("sing")))
		    Dance(true);
    }

    private void AnimationFinished(StringName anim) 
    {
        if (CurrentAnim.PostAnimation != null)
            PlayAnim(CurrentAnim.PostAnimation);
    }
}
