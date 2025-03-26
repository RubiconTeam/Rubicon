using System.Linq;
using Rubicon.Core.API;
using Rubicon.Core.Data;
using Rubicon.Core.Meta;
using Rubicon.Data;
using Rubicon.Game;
using Rubicon.View2D;
using Rubicon.View3D;

namespace Rubicon.API;

/// <summary>
/// A template for a Funkin' styled timer bar in C#.
/// </summary>
[GlobalClass] public abstract partial class CsFunkinTimerBar : CsTimerBar
{
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
        
        InitializeCharacters(RubiconGame.Metadata);
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

        LeftColor = data.Color;
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

        RightColor = data.Color;
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