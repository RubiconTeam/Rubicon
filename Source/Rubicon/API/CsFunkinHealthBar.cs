using System.Linq;
using Rubicon.Core;
using Rubicon.Core.API;
using Rubicon.Core.Data;
using Rubicon.Core.Meta;
using Rubicon.Data;
using Rubicon.Game;
using Rubicon.View2D;

namespace Rubicon.API;

[GlobalClass] public abstract partial class CsFunkinHealthBar : CsHealthBar
{
    /// <summary>
    /// How fast the icon sizes go back to normal.
    /// </summary>
    [Export] public float SizeLerpWeight = 9f;

    /// <summary>
    /// The time of type to go by with <see cref="BounceTime"/>.
    /// </summary>
    [Export] public TimeValue TimeType
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
    /// How often to bounce.
    /// </summary>
    [Export] public float BounceTime
    {
        get
        {
            switch (TimeType)
            {
                case TimeValue.Measure:
                    return _bounceMeasure;
                case TimeValue.Beat:
                    return ConductorUtility.MeasureToBeats(_bounceMeasure);
                case TimeValue.Step:
                    return ConductorUtility.MeasureToSteps(_bounceMeasure);
            }

            return 0; // ????
        }
        set
        {
            switch (TimeType)
            {
                case TimeValue.Measure:
                    _bounceMeasure = value;
                    break;
                case TimeValue.Beat:
                    _bounceMeasure = ConductorUtility.BeatsToMeasures(value);
                    break;
                case TimeValue.Step:
                    _bounceMeasure = ConductorUtility.StepsToMeasures(value);
                    break;
            }

            if (Bumper != null)
                Bumper.Value = value;
        }
    }

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
    [Export] public Control IconContainer;

    public Bumper Bumper;
    
    private SpriteFrames _leftIcon;
    private SpriteFrames _rightIcon;

    private TimeValue _timeType = TimeValue.Beat;
    private float _bounceMeasure = 1f / 4f;
    
    private int _previousHealth = 0;
    private BarDirection _previousDirection = BarDirection.RightToLeft;

    public override void Initialize()
    {
        base.Initialize();

        Bumper = new Bumper();
        Bumper.Type = TimeValue.Measure;
        Bumper.Value = _bounceMeasure;
        Bumper.Name = "Bumper";
        Bumper.Bumped += Bump;
        AddChild(Bumper);
        
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
        
        InitializeCharacters(RubiconGame.Metadata);
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
        if (character is Character2D character2D)
        {
            LeftIcon.SpriteFrames = character2D.Icon;
            LeftColor = character2D.HealthColor;
            LeftIcon.Offset = character2D.IconOffset;
        }
    }
    
    public void SetRightCharacter(Node character)
    {
        if (character is Character2D character2D)
        {
            RightIcon.SpriteFrames = character2D.Icon;
            RightColor = character2D.HealthColor;
            RightIcon.Offset = character2D.IconOffset * new Vector2(-1f, 1f);
        }
    }
    
    private void InitializeCharacters(SongMeta songMeta)
    {
        if (!RubiconGame.Active)
            return;

        switch (songMeta.Environment)
        {
            case GameEnvironment.CanvasItem:
                CanvasItemSpace space = RubiconGame.CanvasItemSpace;
                StringName playerGroupName = RubiconGame.PlayField.TargetBarLine;
                Character2D player = space.GetCharactersFromGroup(playerGroupName).First();
                Character2D opponent = space
                    .GetCharactersFromGroup(RubiconGame.PlayField.BarLines.First(x => x.Name != playerGroupName).Name).First();
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