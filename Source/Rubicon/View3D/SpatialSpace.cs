using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Meta;

namespace Rubicon.View3D;
[GlobalClass]
public partial class SpatialSpace : Node3D
{
    [Export] public RubiconCamera3D Camera;
    [Export] public Array<Character3D> Characters;
    [Export] public Stage3D Stage;
    
    private Dictionary<StringName, CharacterGroup3D> _characterGroups;
    private Dictionary<StringName, Character3D> _namedCharacters;
    private Dictionary<string, PackedScene> _characterScenes;

    public void Initialize(SongMeta meta)
    {
        string stagePath = PathUtility.GetScenePath($"res://Resources/Game/Stages/{meta.Stage}");
        string fallBackStage = ProjectSettings.GetSetting("rubicon/general/fallback/stage_3d").AsString();
        
        if (string.IsNullOrWhiteSpace(stagePath))
        {
            if (meta.Stage == fallBackStage)
            {
                GD.PrintErr($"[SpatialSpace] Fallback stage was not found. Please define a fallback.");
                return;
            }
			
            GD.PrintErr($"[SpatialSpace] Stage {meta.Stage} was not found. Falling back to default.");
            meta.Stage = fallBackStage;
            Initialize(meta);
            return;
        }
        
        Resource stageResource = ResourceLoader.LoadThreadedGet(stagePath);
        if (stageResource is not PackedScene packedScene)
        {
            if (meta.Stage == fallBackStage)
            {
                GD.PrintErr($"[SpatialSpace] Fallback stage was not a PackedScene.");
                return;
            }
			
            GD.PrintErr($"[SpatialSpace] Stage {meta.Stage} is not a PackedScene. Falling back to default.");
            meta.Stage = fallBackStage;
            Initialize(meta);
            return;
        }
        
        Stage = packedScene.Instantiate<Stage3D>();
        if (Stage == null)
        {
            if (meta.Stage == fallBackStage)
            {
                GD.PrintErr($"[SpatialSpace] Fallback stage failed to instantiate.");
                return;
            }
			
            GD.PrintErr($"[SpatialSpace] Stage {meta.Stage} failed to instantiate. Falling back to default.");
            meta.Stage = fallBackStage;
            Initialize(meta);
            return;
        }
        AddChild(Stage);
        
        Camera = new RubiconCamera3D();
        Camera.Name = "Camera";
        AddChild(Camera);

        Camera.TargetFov = Stage.Fov;
        Camera.Fov = Stage.Fov;
        
        Characters = [];
        _namedCharacters = new Dictionary<StringName, Character3D>();
        _characterGroups = new Dictionary<StringName, CharacterGroup3D>();
        _characterScenes = new Dictionary<string, PackedScene>();
        for (int i = 0; i < meta.Characters.Length; i++)
            AddCharacter(meta.Characters[i]);
    }

    public void AddCharacter(CharacterMeta meta)
    {
        string path = PathUtility.GetScenePath($"res://Resources/Game/Characters/{meta.Character}");
        Character3D character = null;
		
        if (_characterScenes.ContainsKey(meta.Character))
        {
            character = _characterScenes[meta.Character].Instantiate<Character3D>();
        }
        else if (!ResourceLoader.Exists(path))
        {
            GD.Print($"[SpatialSpace] Character {meta.Character} was not found. Falling back to default.");
            AddFallbackCharacter(meta);
            return;
        }
        else
        {
            Resource characterResource = ResourceLoader.LoadThreadedGet(path);
            if (characterResource is PackedScene packedScene)
            {
                _characterScenes.Add(meta.Character, packedScene);
                character = packedScene.Instantiate<Character3D>();
            }
            else
            {
                GD.PrintErr($"[SpatialSpace] Character {meta.Character} is not inside a PackedScene. Falling back to default.");
                AddFallbackCharacter(meta);
                return;
            }
        }

        character.Name = meta.Nickname;
        Characters.Add(character);
        _namedCharacters[meta.Nickname] = character;
        Stage.GetSpawnPoint(meta.Nickname).AddCharacter(character);
		
        if (!_characterGroups.ContainsKey(meta.BarLine))
            _characterGroups.Add(meta.BarLine, new CharacterGroup3D());
		
        _characterGroups[meta.BarLine].Characters.Add(character);
    }
    
    public Character3D GetCharacter(StringName nickName) => _namedCharacters[nickName];
	
    public CharacterGroup3D GetCharacterGroup(StringName groupName) => _characterGroups[groupName];

    private void AddFallbackCharacter(CharacterMeta meta)
    {
        string fallBackCharacter = ProjectSettings.GetSetting("rubicon/general/fallback/character_3d").AsString();
        string fallBackPath = PathUtility.GetScenePath($"res://Resources/Game/Characters/{fallBackCharacter}");
        if (!ResourceLoader.Exists(fallBackPath))
        {
            GD.PrintErr("[SpatialSpace] No character fallback was found. Please check your project settings at \"rubicon/general/fallback/character\". Skipping.");
            return;
        }
			
        meta.Character = fallBackCharacter;
        AddCharacter(meta);
    }
}
