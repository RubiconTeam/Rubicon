using System.Linq;
using Rubicon.Core.Data;
using Rubicon.Core.Rulesets;
using Rubicon.Game;
using Rubicon.View2D;

namespace Rubicon.API;

/// <summary>
/// A template for a health bar in C#. Must be inherited.
/// </summary>
[GlobalClass] public abstract partial class CsHealthBar : Control
{
    /// <summary>
    /// What direction the bar filling goes.
    /// </summary>
    [Export] public BarDirection Direction = BarDirection.LeftToRight;
    
    /// <summary>
    /// The bar's color on the left side.
    /// </summary>
    [Export] public Color LeftColor
    {
        get => _leftColor;
        set
        {
            _leftColor = value;
            ChangeLeftColor(value);
        }
    }

    /// <summary>
    /// The bar's color on the right side.
    /// </summary>
    [Export] public Color RightColor
    {
        get => _rightColor;
        set
        {
            _rightColor = value;
            ChangeRightColor(value);
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

    private Color _leftColor = Colors.Red;
    private Color _rightColor = Colors.Green;

    private SpriteFrames _leftIcon;
    private SpriteFrames _rightIcon;
    
    private int _previousHealth = 0;
    private BarDirection _previousDirection = BarDirection.RightToLeft;

    public override void _Ready()
    {
        base._Ready();
        
        PlayField playField = PlayField.Instance;
        if (playField is null)
            return;

        Direction = playField.TargetIndex == 0 ? BarDirection.LeftToRight : BarDirection.RightToLeft;

        if (RubiconGame.Instance is null || playField.Metadata.Environment == GameEnvironment.None)
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
        
        InitializeCharacters();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        PlayField playField = PlayField.Instance;
        if (playField == null || _previousHealth == playField.Health && _previousDirection == Direction)
            return;

        bool playerWinning = playField.Health > Mathf.FloorToInt(playField.MaxHealth * 0.8f);
        bool playerLosing = playField.Health < Mathf.FloorToInt(playField.MaxHealth * 0.2f);
        
        StringName playerAnim = playerWinning ? "win" : playerLosing ? "lose" : "neutral";
        StringName opponentAnim = playerWinning ? "lose" : playerLosing ? "win" : "neutral";
        
        AnimatedSprite2D playerIcon = Direction == BarDirection.LeftToRight ? LeftIcon : RightIcon;
        AnimatedSprite2D opponentIcon = Direction == BarDirection.LeftToRight ? RightIcon : LeftIcon;
        if (playerIcon.SpriteFrames.HasAnimation(playerAnim))
            playerIcon.Play(playerAnim);
            
        if (opponentIcon.SpriteFrames.HasAnimation(opponentAnim))
            opponentIcon.Play(opponentAnim);
        
        UpdateBar((float)playField.Health / playField.MaxHealth, Direction);

        _previousDirection = Direction;
        _previousHealth = playField.Health;
    }
    
    protected abstract void UpdateBar(float progress, BarDirection direction);

    protected abstract void ChangeLeftColor(Color leftColor);
    
    protected abstract void ChangeRightColor(Color rightColor);

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
    
    private void InitializeCharacters()
    {
        RubiconGame game = RubiconGame.Instance;
        if (game is null)
            return;

        switch (game.Metadata.Environment)
        {
            case GameEnvironment.CanvasItem:
                CanvasItemSpace space = game.CanvasItemSpace;
                StringName playerGroupName = game.PlayField.TargetBarLine;
                Character2D player = space.GetCharactersFromGroup(playerGroupName).First();
                Character2D opponent = space
                    .GetCharactersFromGroup(game.PlayField.BarLines.First(x => x.Name != playerGroupName).Name).First();
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