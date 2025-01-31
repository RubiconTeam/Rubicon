using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Meta;
using Rubicon.Core.Rulesets;

namespace Rubicon.View2D;

[GlobalClass]
public partial class CanvasItemSpace : Node2D
{
	[Export] public RubiconCamera2D Camera;

	[Export] public Array<Character2D> Characters;

	[Export] public Stage2D Stage;

	private Dictionary<StringName, Character2D> _namedCharacters;
	private Dictionary<StringName, Array<Character2D>> _barLineCharacters;
	private Dictionary<string, PackedScene> _characterScenes;
	
	public void Initialize(SongMeta meta)
	{
		// Init stage
		string stagePath = PathUtility.GetScenePath($"res://Resources/Stages/{meta.Stage}");
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
		if (Stage.SnapZoomOnStart)
			Camera.Zoom = Stage.Zoom;
		
		// Init characters
		Characters = new Array<Character2D>();
		_namedCharacters = new Dictionary<StringName, Character2D>();
		_barLineCharacters = new Dictionary<StringName, Array<Character2D>>();
		_characterScenes = new Dictionary<string, PackedScene>();
		for (int i = 0; i < meta.Characters.Length; i++)
			AddCharacter(meta.Characters[i]);
	}

	public void AddCharacter(CharacterMeta meta)
	{
		string path = PathUtility.GetScenePath($"res://Resources/Characters/{meta.Character}");
		Character2D character = null;
		
		if (_characterScenes.ContainsKey(meta.Character))
		{
			character = _characterScenes[meta.Character].Instantiate<Character2D>();
		}
		else if (!ResourceLoader.Exists(path))
		{
			GD.Print($"[CanvasItemSpace] Character {meta.Character} was not found. Falling back to default.");
			AddFallbackCharacter(meta);
			return;
		}
		else
		{
			Resource characterResource = ResourceLoader.LoadThreadedGet(path);
			if (characterResource is PackedScene packedScene)
			{
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
		
		if (!_barLineCharacters.ContainsKey(meta.BarLine))
			_barLineCharacters.Add(meta.BarLine, new Array<Character2D>());
		
		_barLineCharacters[meta.BarLine].Add(character);
	}

	public Character2D GetCharacter(StringName nickName) => _namedCharacters[nickName];
	
	public Array<Character2D> GetCharactersFromGroup(StringName groupName) => _barLineCharacters[groupName];

	
	public void SingForGroup(StringName barLineName, string direction, bool holding = false, bool miss = false, string customPrefix = null, string customSuffix = null)
	{
		Array<Character2D> characters = GetCharactersFromGroup(barLineName);
		for (int i = 0; i < characters.Count; i++)
			characters[i].Sing(direction, holding, miss, customPrefix, customSuffix);
	}

	public Vector2 GetGroupCameraPosition(StringName groupName)
	{
		Array<Character2D> characters = GetCharactersFromGroup(groupName);
		if (characters.Count < 1)
			return Vector2.Zero;

		Vector2 min = characters[0].GetCameraPosition();
		Vector2 max = characters[0].GetCameraPosition();
		
		for (int i = 1; i < characters.Count; i++)
		{
			Character2D character = characters[i];
			Vector2 camPos = character.GetCameraPosition();
			
			min.X = Math.Min(camPos.X, min.X);
			min.Y = Math.Min(camPos.Y, min.Y);
			max.X = Math.Max(max.X, camPos.X);
			max.Y = Math.Max(max.Y, camPos.Y);
		}
		
		return new Vector2(min.X + (max.X - min.X) / 2f, min.Y + (max.Y - min.Y) / 2f);
	}

	private void AddFallbackCharacter(CharacterMeta meta)
	{
		string fallBackCharacter = ProjectSettings.GetSetting("rubicon/general/fallback/character_2d").AsString();
		string fallBackPath = PathUtility.GetScenePath($"res://Resources/Characters/{fallBackCharacter}");
		if (!ResourceLoader.Exists(fallBackPath)) // Bro
		{
			GD.PrintErr("[CanvasItemSpace] No character fallback was found. Please check your project settings at \"rubicon/general/fallback/character\". Skipping.");
			return;
		}
			
		meta.Character = fallBackCharacter;
		AddCharacter(meta);
	}
}
