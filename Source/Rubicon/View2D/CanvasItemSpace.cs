using Godot.Collections;
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
		Stage = (ResourceLoader.LoadThreadedGet($"res://Resources/Stages/{meta.Stage}.tscn") as PackedScene).Instantiate<Stage2D>(); // TODO: Check if stage even exists
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
		string path = $"res://Resources/Characters/{meta.Character}.tscn";
		Character2D character = null;
		GD.Print(meta.Nickname, " ", path);
		
		if (_characterScenes.ContainsKey(meta.Character))
		{
			character = _characterScenes[meta.Character].Instantiate<Character2D>();
		}
		else if (!ResourceLoader.Exists(path))
		{
			GD.Print($"Character {meta.Character} was not found. Falling back to default.");
			string fallBackCharacter = ProjectSettings.GetSetting("rubicon/general/fallback/character").AsString();
			string fallBackPath = $"res://Resources/Characters/{fallBackCharacter}.tscn";
			if (!ResourceLoader.Exists(fallBackPath)) // Bro
			{
				GD.PrintErr("No character fallback was found. Please check your project settings at \"rubicon/general/fallback/character\". Skipping.");
				return;
			}

			Resource characterResource = ResourceLoader.LoadThreadedGet(fallBackPath);
			if (characterResource is PackedScene packedScene)
			{
				_characterScenes.Add(meta.Character, packedScene);
				character = packedScene.Instantiate<Character2D>();
			}
		}
		else
		{
			Resource characterResource = ResourceLoader.LoadThreadedGet(path);
			if (characterResource is PackedScene packedScene)
			{
				_characterScenes.Add(meta.Character, packedScene);
				character = packedScene.Instantiate<Character2D>();
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
}
