using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Meta;
using Rubicon.View3D;

namespace Rubicon.View2D;

[GlobalClass]
public partial class CanvasItemSpace : Node2D
{
	[Export] public RubiconCamera2D Camera;

	[Export] public Array<Character2D> Characters;

	[Export] public Stage2D Stage;

	private Dictionary<StringName, CharacterGroup2D> _characterGroups;
	private Dictionary<StringName, Character2D> _namedCharacters;
	private Dictionary<string, PackedScene> _characterScenes;
	
	public void Initialize(SongMeta meta)
	{
		// Init stage
		string stagePath = PathUtility.GetScenePath($"res://Resources/Game/Stages/{meta.Stage}");
		string fallBackStage = ProjectSettings.GetSetting("rubicon/general/fallback/stage_2d").AsString();
		if (string.IsNullOrWhiteSpace(stagePath))
		{
			if (meta.Stage == fallBackStage)
			{
				GD.PrintErr($"[CanvasItemSpace] Fallback stage was not found. Please define a fallback.");
				return;
			}
			
			GD.PrintErr($"[CanvasItemSpace] Stage {meta.Stage} was not found. Falling back to default.");
			meta.Stage = fallBackStage;
			Initialize(meta);
			return;
		}

		Resource stageResource = ResourceLoader.LoadThreadedGet(stagePath);
		if (stageResource is not PackedScene packedScene)
		{
			if (meta.Stage == fallBackStage)
			{
				GD.PrintErr($"[CanvasItemSpace] Fallback stage was not a PackedScene.");
				return;
			}
			
			GD.PrintErr($"[CanvasItemSpace] Stage {meta.Stage} is not a PackedScene. Falling back to default.");
			meta.Stage = fallBackStage;
			Initialize(meta);
			return;
		}

		Stage = packedScene.Instantiate<Stage2D>();
		if (Stage == null)
		{
			if (meta.Stage == fallBackStage)
			{
				GD.PrintErr($"[CanvasItemSpace] Fallback stage failed to instantiate.");
				return;
			}
			
			GD.PrintErr($"[CanvasItemSpace] Stage {meta.Stage} failed to instantiate. Falling back to default.");
			meta.Stage = fallBackStage;
			Initialize(meta);
			return;
		}
		
		AddChild(Stage);

		Camera = new RubiconCamera2D();
		Camera.Name = "Camera";
		AddChild(Camera);

		Camera.TargetZoom = Stage.Zoom;
		Camera.Zoom = Stage.Zoom;
		
		// Init characters
		Characters = [];
		_namedCharacters = new Dictionary<StringName, Character2D>();
		_characterGroups = new Dictionary<StringName, CharacterGroup2D>();
		_characterScenes = new Dictionary<string, PackedScene>();
		for (int i = 0; i < meta.Characters.Length; i++)
			AddCharacter(meta.Characters[i]);
	}

	public void AddCharacter(CharacterMeta meta)
	{
		string path = PathUtility.GetScenePath($"res://Resources/Game/Characters/{meta.Character}");
		Character2D character = null;
		
		if (_characterScenes.ContainsKey(meta.Character))
		{
			character = _characterScenes[meta.Character].Instantiate<Character2D>();
		}
		else if (!ResourceLoader.Exists(path))
		{
			GD.PrintErr($"[CanvasItemSpace] Character {meta.Character} was not found. Falling back to default.");
			AddFallbackCharacter(meta);
			return;
		}
		else
		{
			Resource characterResource = ResourceLoader.LoadThreadedGet(path);
			if (characterResource is PackedScene packedScene)
			{
				Node characterInstance = packedScene.Instantiate();
				if (characterInstance is Character3D)
				{
					GD.PrintErr($"[CanvasItemSpace] Character {meta.Character} is a 3D character. Falling back to default.");
					AddFallbackCharacter(meta);
					return;
				}
				
				_characterScenes.Add(meta.Character, packedScene);
				character = packedScene.Instantiate<Character2D>();
			}
			else
			{
				GD.PrintErr($"[CanvasItemSpace] Character {meta.Character} is not inside a PackedScene. Falling back to default.");
				AddFallbackCharacter(meta);
				return;
			}
		}

		character.Name = meta.Nickname;
		Characters.Add(character);
		_namedCharacters[meta.Nickname] = character;
		Stage.GetSpawnPoint(meta.Nickname).AddCharacter(character);
		
		if (!_characterGroups.ContainsKey(meta.BarLine))
			_characterGroups.Add(meta.BarLine, new CharacterGroup2D());
		
		_characterGroups[meta.BarLine].Characters.Add(character);
	}

	public Character2D GetCharacter(StringName nickName) => _namedCharacters[nickName];
	
	public CharacterGroup2D GetCharacterGroup(StringName groupName) => _characterGroups[groupName];

	private void AddFallbackCharacter(CharacterMeta meta)
	{
		string fallBackCharacter = ProjectSettings.GetSetting("rubicon/general/fallback/character_2d").AsString();
		string fallBackPath = PathUtility.GetScenePath($"res://Resources/Game/Characters/{fallBackCharacter}");
		if (!ResourceLoader.Exists(fallBackPath)) // Bro
		{
			GD.PrintErr("[CanvasItemSpace] No character fallback was found. Please check your project settings at \"rubicon/general/fallback/character\". Skipping.");
			return;
		}
			
		meta.Character = fallBackCharacter;
		AddCharacter(meta);
	}
}
