using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Meta;
using Rubicon.View2D;

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

    public bool Initialized = false;

    public void Initialize(SongMeta meta)
    {
        // Init stage
        string stagePath = PathUtility.GetScenePath($"res://Resources/Game/Stages/{meta.Stage}");
        string fallBackStage = ProjectSettings.GetSetting("rubicon/general/fallback/stage_3d").AsString();
        if (string.IsNullOrWhiteSpace(stagePath))
        {
            if (meta.Stage == fallBackStage)
                throw new Exception($"Fallback stage was not found. Please define a valid fallback.");

            PrintUtility.PrintError("SpatialSpace", $"Stage \"{meta.Stage}\" was not found. Falling back to default.");
            meta.Stage = fallBackStage;
            Initialize(meta);
            return;
        }

        Resource stageResource = ResourceLoader.LoadThreadedGet(stagePath);
        if (stageResource is not PackedScene packedScene)
        {
            if (meta.Stage == fallBackStage)
                throw new Exception($"Fallback stage \"{fallBackStage}\" was not a PackedScene.");
			
            PrintUtility.PrintError("SpatialSpace", $"Stage \"{meta.Stage}\" is not a PackedScene. Falling back to default.");
            meta.Stage = fallBackStage;
            Initialize(meta);
            return;
        }

        Stage = packedScene.Instantiate<Stage3D>();
        if (Stage == null)
        {
            if (meta.Stage == fallBackStage)
                throw new Exception($"Fallback stage \"{fallBackStage}\" failed to instantiate.");
			
            PrintUtility.PrintError("SpatialSpace", $"Stage \"{meta.Stage}\" failed to instantiate. Falling back to default.");
            meta.Stage = fallBackStage;
            Initialize(meta);
            return;
        }
        PrintUtility.Print("SpatialSpace", $"Loaded stage: {meta.Stage}.", true);
        PrintUtility.Print("SpatialSpace", $"Loaded stage: {meta.Stage}.", true);
        
        Camera = new RubiconCamera3D();
        Camera.Name = "Camera";
        Camera.Current = true;
        AddChild(Camera);

        Camera.TargetFov = Stage.Fov;
        Camera.Fov = Stage.Fov;
        
        Characters = [];
        _namedCharacters = new Dictionary<StringName, Character3D>();
        _characterGroups = new Dictionary<StringName, CharacterGroup3D>();
        _characterScenes = new Dictionary<string, PackedScene>();
        for (int i = 0; i < meta.Characters.Length; i++)
            AddCharacter(meta.Characters[i]);
        
        Initialized = true;
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
            PrintUtility.PrintError("SpatialSpace", $"Character \"{meta.Character}\" was not found. Falling back to default.");
            AddFallbackCharacter(meta);
            return;
        }
        else
        {
            Resource characterResource = ResourceLoader.LoadThreadedGet(path);
            if (characterResource is PackedScene packedScene)
            {
                Node characterInstance = packedScene.Instantiate();
                if (characterInstance is Character2D)
                {
                    PrintUtility.PrintError("SpatialSpace", $"Character \"{meta.Character}\" is not a 3D character. Falling back to default.");
                    AddFallbackCharacter(meta);
                    return;
                }
				
                _characterScenes.Add(meta.Character, packedScene);
                character = packedScene.Instantiate<Character3D>();
            }
            else
            {
                PrintUtility.PrintError("SpatialSpace", $"Character \"{meta.Character}\" is not inside a PackedScene. Falling back to default.");
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
        PrintUtility.Print("SpatialSpace", $"Added Character: {meta.Character}", true);
    }
    
    public Character3D GetCharacter(StringName nickName) => _namedCharacters[nickName];
	
    public CharacterGroup3D GetCharacterGroup(StringName groupName) => _characterGroups[groupName];

    private void AddFallbackCharacter(CharacterMeta meta)
    {
        string fallBackCharacter = ProjectSettings.GetSetting("rubicon/general/fallback/character_3d").AsString();
        string fallBackPath = PathUtility.GetScenePath($"res://Resources/Game/Characters/{fallBackCharacter}");
        if (!ResourceLoader.Exists(fallBackPath))
        {
            PrintUtility.PrintError("SpatialSpace", "No character fallback was found. Please check your project settings at \"rubicon/general/fallback/character\". Skipping.");
            return;
        }
			
        meta.Character = fallBackCharacter;
        AddCharacter(meta);
    }
}
