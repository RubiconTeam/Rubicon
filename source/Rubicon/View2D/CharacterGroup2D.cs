using Godot.Collections;
using Rubicon.Data;

namespace Rubicon.View2D;

/// <summary>
/// Group of multiple <see cref="Character2D"/>'s which can be used
/// to react to the same Sing and Dance calls among other utilities.
/// </summary>
[GlobalClass] public partial class CharacterGroup2D : Node2D
{
    /// <summary>
    /// The array of characters in the group
    /// </summary>
    [Export] public Array<Character2D> Characters = [];
    
    public void SetGlobalPrefix(string prefix)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].GlobalPrefix = prefix;
    }

    public void SetGlobalSuffix(string suffix)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].GlobalSuffix = suffix;
    }

    public void SetFreezeSinging(bool freeze)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].FreezeSinging = freeze;
    }

    public void SetCanDance(bool canDance)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].FreezeDance = canDance;
    }

    public Vector2 GetCameraPoint()
    {
        if (Characters.Count < 1)
            return Vector2.Zero;

        Vector2 min = Characters[0].GetCameraPosition();
        Vector2 max = Characters[0].GetCameraPosition();
		
        for (int i = 1; i < Characters.Count; i++)
        {
            Character2D character = Characters[i];
            Vector2 camPos = character.GetCameraPosition();
			
            min.X = Math.Min(camPos.X, min.X);
            min.Y = Math.Min(camPos.Y, min.Y);
            max.X = Math.Max(max.X, camPos.X);
            max.Y = Math.Max(max.Y, camPos.Y);
        }
		
        return new Vector2(min.X + (max.X - min.X) / 2f, min.Y + (max.Y - min.Y) / 2f);
    }

    public void Dance(string customPrefix = null, string customSuffix = null)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].Dance(customPrefix, customSuffix);
    }

    public void Sing(string direction, bool holding = false, bool miss = false, string customPrefix = null, string customSuffix = null)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].Sing(direction, holding, miss, customPrefix, customSuffix);
    }
    
    public void PlaySpecialAnimation(SpecialAnimation anim)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].PlaySpecialAnimation(anim);
    }
}