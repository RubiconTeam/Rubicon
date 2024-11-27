using Rubicon.Core;
using Rubicon.Data;

namespace Rubicon.View2D;

/// <summary>
/// This is the character class, which handles which animations the sprite should play
/// among other utils
/// </summary>
public partial class Character2D : Node2D
{
    /// <summary>
    /// Determines whether the character is facing left or not.
    /// </summary>
    [ExportGroup("Character Info"), Export] public bool LeftFacing = false;

    /// <summary>
    /// This property is automatically determined by the character class.
    /// It flips the left and right animations depending on if <see cref="LeftFacing"/> does not match the <see cref="SpawnPoint2D"/>'s <see cref="SpawnPoint2D.LeftFacing"/>
    /// </summary>
    public bool FlipAnimations = false;

    /// <summary>
    /// This property gets added to the start of an animation's name.
    /// It is overriden by the <see cref="CharacterAnimation.CustomPrefix"/> field inside <see cref="CurrentAnim"/>.
    /// Useful for alt animations or similar.
    /// </summary>
    [ExportGroup("Animation Info"), Export] public string GlobalPrefix;

    /// <summary>
    /// This property gets added to the end of an animation's name.
    /// It is overriden by the <see cref="CharacterAnimation.CustomSuffix"/> field inside <see cref="CurrentAnim"/>.
    /// Useful for alt animations or similar.
    /// </summary>
    [Export] public string GlobalSuffix;
    
    /// <summary>
    /// A string array containing the sequence of idle/dance animations to be played.
    /// Useful for left to right dance animations.
    /// </summary>
    [Export] public string[] DanceList = ["idle"];

    /// <summary>
    /// The index for which dance animation in the <see cref="DanceList"/> should play.
    /// </summary>
    public int DanceIndex = 0;

    public double DanceMeasure = 1f / 2f;

    /// <summary>
    /// If <see langword="true"/>, the character will jitter when holding a note. If <see langword="false"/>, it will stay completely static.
    /// </summary>
    [Export] public bool StaticSustain = false;

    /// <summary>
    /// The animation that is currently being played.
    /// </summary>
    public CharacterAnimation CurrentAnim = null;

    /// <summary>
    /// The animation that was last played.
    /// </summary>
    public CharacterAnimation LastAnim = null;

    /// <summary>
    /// The duration of the sing animations before going back to idle.
    /// </summary>
    [Export] public float SingDuration = 4;

    /// <summary>
    /// A timer that determines if the animation should be finished or not.
    /// </summary>
    public double SingTimer;

    /// <summary>
    /// A timer that determines if the hold animation should be repeated or not.
    /// It will not be used in case <paramref name="StaticSustain"/> is <see langword="true"/>.
    /// </summary>
    public float HoldTimer;

    public bool Holding = false;

    /// <summary>
    /// The <see cref="SpriteFrames"/>> used for the healthbar icons.
    /// It has to contain an idle animation.
    /// It can contain "lose" and "win" optionally.
    /// </summary>
    [ExportGroup("Healthbar Info"), Export] public SpriteFrames CharacterIcon { get; set; } = GD.Load<SpriteFrames>("res://Assets/Characters/Placeholder/Icon.tres");

    /// <summary>
    /// The offset of the healthbar icon.
    /// </summary>
    [Export] public Vector2 IconOffset = new Vector2(0,10);

    /// <summary>
    /// The healthbar color of this character.
    /// </summary>
    [Export] public Color HealthColor = new("#A1A1A1");

    /// <summary>
    /// The main <see cref="Node2D"/> used for positioning and scaling.
    /// Usually its the main <see cref="PlayerSprite2D"/> node.
    /// </summary>
    [ExportGroup("Path Info"), Export] public Node2D MainNode;

    /// <summary>
    /// The main <see cref="AnimationPlayer"> node used for playing animations.
    /// </summary>
    [Export] public AnimationPlayer AnimPlayer;

    /// <summary>
    /// A <see cref="Marker2D"/> from which the camera takes its position.
    /// </summary>
    [Export] public Marker2D CameraPoint;

    private int _lastStep = -int.MaxValue;
    private double _lastDanceSnap = double.NegativeInfinity;

    public override void _Ready()
    {
        base._Ready();

        AnimPlayer.AnimationFinished += AnimationFinished;
        
        // These should be determined by the stage
        /*
        FlipAnimations = IsPlayer != MirrorCharacter;
		if (IsPlayer != MirrorCharacter) Scale *= new Vector2(-1,1);*/
    }

    public override void _Process(double delta)
    {
	    base._Process(delta);
	    
	    // Hacky hold method, should work for now
	    int curStep = Mathf.FloorToInt(Conductor.CurrentStep);
	    if (Holding)
	    {
		    // For FNF jitter
		    if (_lastStep != curStep)
		    {
			    AnimPlayer.Seek(0f);
			    SingTimer = 0;
		    }
	    }

	    double curDanceSnap = Mathf.Snapped(Conductor.CurrentMeasure, DanceMeasure);
	    if ((CurrentAnim.Name.StartsWith("sing") && SingTimer >= SingDuration || !CurrentAnim.Name.StartsWith("sing")) && !Mathf.IsEqualApprox(curDanceSnap, _lastDanceSnap))
		    Dance();

	    SingTimer += delta;
	    _lastStep = curStep;
	    _lastDanceSnap = curDanceSnap;
    }
    
    public void Dance(bool force = false)
    {
	    if (!force && AnimPlayer.CurrentAnimationPosition < AnimPlayer.CurrentAnimationLength) 
		    return;

	    PlayAnim(DanceList[DanceIndex], true);

	    DanceIndex++;
	    DanceIndex = Mathf.Wrap(DanceIndex, 0, DanceList.Length-1);
    }

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
	    if (AnimPlayer.HasAnimation(animName + "-post"))
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

    public virtual void PlayAnim(string anim, bool force = false, float startTime = 0f, string customPrefix = null, string customSuffix = null)
    {
	    PlayAnim(new CharacterAnimation
	    {
		    Name = anim,
		    Force = force,
		    StartTime = startTime,
		    CustomPrefix = customPrefix,
		    CustomSuffix = customSuffix
	    });
    }

    public void PlayAnim(CharacterAnimation anim)
    {
	    if (!anim.Force && AnimPlayer.CurrentAnimationPosition >= AnimPlayer.CurrentAnimationLength)
		    return;

	    string originalName = anim.Name; 
	    if (FlipAnimations) 
		    anim.Name = anim.Name.Contains("LEFT") ? anim.Name.Replace("LEFT", "RIGHT") : anim.Name.Replace("RIGHT", "LEFT");

	    string prefix = !string.IsNullOrEmpty(anim.CustomPrefix) ? anim.CustomPrefix : GlobalPrefix;
	    string suffix = !string.IsNullOrEmpty(anim.CustomSuffix) ? anim.CustomSuffix : GlobalSuffix;
	    
	    anim.Name = $"{prefix}{anim.Name}{suffix}";
	    if (!AnimPlayer.HasAnimation(anim.Name))
	    {
		    GD.PushWarning($"There is no animation in the AnimationPlayer called {anim.Name}. (Original Animation: {originalName})");
		    return;
	    }
	    
	    LastAnim = CurrentAnim;
	    CurrentAnim = anim;
	    AnimPlayer.Play(anim.Name);
	    AnimPlayer.Seek(anim.StartTime);
    }

    public void AnimationFinished(StringName anim) 
    {
        if(CurrentAnim.PostAnimation != null)
            PlayAnim(CurrentAnim.PostAnimation);
    }
}
