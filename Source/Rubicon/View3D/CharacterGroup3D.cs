using Rubicon.Data;
using Godot.Collections;

namespace Rubicon.View3D;
[GlobalClass] public partial class CharacterGroup3D : Node3D
{
    [Export] public Array<Character3D> Characters = [];
    
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

    public Vector3 GetCameraPosition()
    {
        if (Characters.Count < 1)
            return Vector3.Zero;

        Vector3 min = Characters[0].GetCameraPosition();
        Vector3 max = Characters[0].GetCameraPosition();
		
        for (int i = 1; i < Characters.Count; i++)
        {
            Character3D character = Characters[i];
            Vector3 camPos = character.GetCameraPosition();
			
            min.X = Math.Min(camPos.X, min.X);
            min.Y = Math.Min(camPos.Y, min.Y);
            min.Z = Math.Min(camPos.Z, min.Z);
            
            max.X = Math.Max(max.X, camPos.X);
            max.Y = Math.Max(max.Y, camPos.Y);
            max.Z = Math.Max(max.Z, camPos.Z);
        }
		
        return new Vector3(min.X + (max.X - min.X) / 2f, min.Y + (max.Y - min.Y) / 2f, min.Z + (max.Z - min.Z) / 2f);
    }
    
    public Vector3 GetCameraRotation()
    {
        if (Characters.Count < 1)
            return Vector3.Zero;

        Vector3 rotation = Characters[0].GetCameraRotation();
		
        return rotation;
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
