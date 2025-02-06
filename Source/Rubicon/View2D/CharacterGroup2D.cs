using Godot.Collections;
using Rubicon.Data;

namespace Rubicon.View2D;

[GlobalClass] public partial class CharacterGroup2D : Node2D
{
    [Export] public Array<Character2D> Characters = new();
    
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
            Characters[i].CanDance = canDance;
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

    public void Dance(bool force = false)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].Dance(force);
    }

    public void Sing(string direction, bool holding = false, bool miss = false)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].Sing(direction, holding, miss);
    }

    public void SingWithCustomAnimation(CharacterAnimation anim, bool holding = false)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].SingWithCustomAnimation(anim, holding);
    }

    public void PlayAnimation(CharacterAnimation anim)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].PlayAnimation(anim);
    }

    public void PlayAnimationWithName(string animName, bool force = false, float startTime = 0f)
    {
        for (int i = 0; i < Characters.Count; i++)
            Characters[i].PlayAnimationWithName(animName, force, startTime);
    }
}