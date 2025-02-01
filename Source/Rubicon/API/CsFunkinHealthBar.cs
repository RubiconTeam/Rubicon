using System.Linq;
using Rubicon.Core.API;
using Rubicon.Core.Data;
using Rubicon.Data;
using Rubicon.Game;
using Rubicon.View2D;

namespace Rubicon.API;

[GlobalClass] public abstract partial class CsFunkinHealthBar : CsHealthBar
{
    /// <summary>
    /// The icon on the left side.
    /// </summary>
    [Export] public AnimatedSprite2D LeftIcon;

    /// <summary>
    /// The icon on the right side.
    /// </summary>
    [Export] public AnimatedSprite2D RightIcon;
    
    private SpriteFrames _leftIcon;
    private SpriteFrames _rightIcon;

    public override void Initialize()
    {
        base.Initialize();
        
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
        
        InitializeCharacters();
    }

    public override void _Process(double delta)
    {
        if (PlayField == null || PreviousHealth == PlayField.Health && PreviousDirection == Direction)
            return;
        
        base._Process(delta);
        
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
    
    private void InitializeCharacters()
    {
        if (!RubiconGame.Active)
            return;

        switch (RubiconGame.Metadata.Environment)
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