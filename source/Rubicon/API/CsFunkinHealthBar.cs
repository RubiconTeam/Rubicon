using System.Linq;
using Rubicon.Core;
using Rubicon.Core.API;
using Rubicon.Core.Data;
using Rubicon.Core.Meta;
using Rubicon.Data;
using Rubicon.Game;
using Rubicon.View2D;
using Rubicon.View3D;

namespace Rubicon.API;

/// <summary>
/// A template for a classic Funkin' styled health bar in C#.
/// </summary>
[GlobalClass] public abstract partial class CsFunkinHealthBar : CsHealthBar
{
    /// <summary>
    /// How fast the icon sizes go back to normal.
    /// </summary>
    [Export] public float SizeLerpWeight = 9f;
    
    /// <summary>
    /// The icon on the left side.
    /// </summary>
    [Export] public AnimatedSprite2D LeftIcon;

    /// <summary>
    /// The icon on the right side.
    /// </summary>
    [Export] public AnimatedSprite2D RightIcon;

    /// <summary>
    /// The container for both icons.
    /// </summary>
    [Export] public PathFollow2D IconContainer;

    /// <summary>
    /// The time of type to go by with <see cref="BounceTime"/>.
    /// </summary>
    [ExportGroup("Time")] public TimeValue TimeType
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
    /// How often to bounce.
    /// </summary>
    [Export] public float BounceTime
    {
        get
        {
            if (BeatSyncer != null)
                return BeatSyncer.Value;

            return _bounceValue;
        }
        set
        {
            if (BeatSyncer != null)
                BeatSyncer.Value = value;
            
            _bounceValue = value;
        }
    }

    /// <summary>
    /// The node that controls when the icons bounce.
    /// </summary>
    public BeatSyncer BeatSyncer;
    
    private float _bounceValue = 1f / 4f;
    private TimeValue _timeType = TimeValue.Measure;
    
    private int _previousHealth = 0;
    private BarDirection _previousDirection = BarDirection.RightToLeft;

    public override void Initialize()
    {
        base.Initialize();

        BeatSyncer = new BeatSyncer();
        BeatSyncer.Type = _timeType;
        BeatSyncer.Value = _bounceValue;
        BeatSyncer.Name = "Bumper";
        BeatSyncer.Bumped += Bump;
        AddChild(BeatSyncer);
        
        if (!RubiconGame.Active || RubiconGame.Metadata.Environment == GameEnvironment.None)
        {
            if (Direction == BarDirection.LeftToRight)
            {
                LeftColor = Colors.Green;
                RightColor = Colors.Red;
            }
            else if (Direction == BarDirection.RightToLeft)
            {
                LeftColor = Colors.Red;
                RightColor = Colors.Green;
            }

            return;
        }
        
        InitializeCharacters(PlayField.Metadata);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Vector2 scale = IconContainer.Scale;
        if (!scale.IsEqualApprox(Vector2.One))
        {
            Vector2 nextScale = scale.Lerp(Vector2.One, SizeLerpWeight * (float)delta);
            IconContainer.Scale = nextScale.IsEqualApprox(Vector2.One) ? Vector2.One : nextScale;
        }
        
        if (PlayField == null || _previousHealth == PlayField.Health && _previousDirection == Direction)
            return;

        float value = Direction != BarDirection.LeftToRight ? 1f - ProgressRatio : ProgressRatio;
        IconContainer.ProgressRatio = value;
        
        bool playerWinning = PlayField.Health > Mathf.FloorToInt(PlayField.MaxHealth * 0.8f);
        bool playerLosing = PlayField.Health < Mathf.FloorToInt(PlayField.MaxHealth * 0.2f);
        
        StringName playerAnim = playerWinning ? "win" : playerLosing ? "lose" : "neutral";
        StringName opponentAnim = playerWinning ? "lose" : playerLosing ? "win" : "neutral";
        
        AnimatedSprite2D playerIcon = Direction == BarDirection.LeftToRight ? LeftIcon : RightIcon;
        AnimatedSprite2D opponentIcon = Direction == BarDirection.LeftToRight ? RightIcon : LeftIcon;
        
        if (playerIcon is not null && playerIcon.SpriteFrames.HasAnimation(playerAnim))
            playerIcon.Play(playerAnim);
        
        if (opponentIcon is not null && opponentIcon.SpriteFrames.HasAnimation(opponentAnim))
            opponentIcon.Play(opponentAnim);
        
        _previousDirection = Direction;
        _previousHealth = PlayField.Health;
    }

    public void Bump()
    {
        IconContainer.Scale = Vector2.One * 1.2f;
    }
    
    public void SetLeftCharacter(Node character)
    {
        CharacterIconData data = null;
        if (character is Character2D character2D)
            data = character2D.Icon;
        else if (character is Character3D character3D)
            data = character3D.Icon;
            
        if (data is null)
            return;

        LeftIcon.SpriteFrames = data.Icon;
        LeftColor = data.Color;
        LeftIcon.Offset = data.Offset;
        LeftIcon.Scale = data.Scale;
        LeftIcon.TextureFilter = data.Filter;
    }
    
    public void SetRightCharacter(Node character)
    {
        CharacterIconData data = null;
        if (character is Character2D character2D)
            data = character2D.Icon;
        else if (character is Character3D character3D)
            data = character3D.Icon;

        if (data is null)
            return;

        RightIcon.SpriteFrames = data.Icon;
        RightColor = data.Color;
        RightIcon.Offset = data.Offset * new Vector2(-1f, 1f);;
        RightIcon.Scale = data.Scale;
        RightIcon.TextureFilter = data.Filter;
    }
    
    private void InitializeCharacters(SongMeta songMeta)
    {
        if (!RubiconGame.Active)
            return;

        switch (songMeta.Environment)
        {
            case GameEnvironment.CanvasItem:
                CanvasItemSpace canvasItemSpace = RubiconGame.CanvasItemSpace;
                StringName canvasPlayerGroupName = PlayField.TargetBarLine;
                Character2D canvasPlayer = canvasItemSpace.GetCharacterGroup(canvasPlayerGroupName).Characters.First();
                Character2D canvasOpponent = canvasItemSpace
                    .GetCharacterGroup(PlayField.BarLines.First(x => x.Name != canvasPlayerGroupName).Name)
                    .Characters.First();
                
                if (Direction == BarDirection.LeftToRight)
                {
                    SetLeftCharacter(canvasPlayer);
                    SetRightCharacter(canvasOpponent);
                }
                else if (Direction == BarDirection.RightToLeft)
                {
                    SetLeftCharacter(canvasOpponent);
                    SetRightCharacter(canvasPlayer);
                }
                break;
            case GameEnvironment.Spatial:
                SpatialSpace spatialSpace = RubiconGame.SpatialSpace;
                StringName spatialPlayerGroupName = PlayField.TargetBarLine;
                Character3D player = spatialSpace.GetCharacterGroup(spatialPlayerGroupName).Characters.First();
                Character3D opponent = spatialSpace
                    .GetCharacterGroup(PlayField.BarLines.First(x => x.Name != spatialPlayerGroupName).Name)
                    .Characters.First();
                
                if (Direction == BarDirection.LeftToRight)
                {
                    SetLeftCharacter(player);
                    SetRightCharacter(opponent);
                }
                else if (Direction == BarDirection.RightToLeft)
                {
                    SetLeftCharacter(opponent);
                    SetRightCharacter(player);
                }
                break;
        }
    }
}