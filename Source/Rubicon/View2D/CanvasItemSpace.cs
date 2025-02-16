using Godot.Collections;
using Rubicon.Core;
using Rubicon.Core.Meta;
using Rubicon.View3D;

namespace Rubicon.View2D;

[GlobalClass]
public partial class CanvasItemSpace : Node2D
{
	/// <summary>
	/// The <see cref="RubiconCamera2D"/> node inside the space.
	/// </summary>
	[Export] public RubiconCamera2D Camera;

	/// <summary>
	/// An array of 2D characters used in the song.
	/// Determined by <see cref="SongMeta"/>.
	/// </summary>
	[Export] public Array<Character2D> Characters;

	/// <summary>
	/// The stage used in the song.
	/// Determined by <see cref="SongMeta"/>.
	/// </summary>
	[Export] public Stage2D Stage;

	private Dictionary<StringName, CharacterGroup2D> _characterGroups;
	private Dictionary<StringName, Character2D> _namedCharacters;
	private Dictionary<string, PackedScene> _characterScenes;
	
	/// <summary>
	/// Determines if the environment has been fully loaded.
	/// Equals <see langword="false"/> if there's any exception while initializing.
	/// </summary>
	public bool Initialized = false;
	
	/// <summary>
	/// Loads all characters and stages found in <see cref="SongMeta"/>.
	/// If a character or stage was not found, it will load its fallback instead.
	/// </summary>
	public void Initialize(SongMeta meta)
	{
		// Init stage
		string stagePath = PathUtility.GetScenePath($"res://Resources/Game/Stages/{meta.Stage}");
		string fallBackStage = ProjectSettings.GetSetting("rubicon/general/fallback/stage_2d").AsString();
		if (string.IsNullOrWhiteSpace(stagePath))
		{
			if (meta.Stage == fallBackStage)
				throw new Exception($"Fallback stage was not found. Please define a valid fallback.");
			
			PrintUtility.PrintError("CanvasItemSpace", $"Stage was not found. Falling back to default.");
			meta.Stage = fallBackStage;
			Initialize(meta);
			return;
		}

		Resource stageResource = ResourceLoader.LoadThreadedGet(stagePath);
		if (stageResource is PackedScene packedScene)
		{
			Node stageInstance = packedScene.Instantiate();
			if (stageInstance is not Stage2D)
			{
				if (meta.Stage == fallBackStage)
					throw new Exception($"Fallback stage is not a 2D stage.");

				PrintUtility.PrintError("CanvasItemSpace", $"Stage \"{meta.Stage}\" is not a 2D stage. Falling back to default.");
				meta.Stage = fallBackStage;
				Initialize(meta);
				return;
			}
			Stage = packedScene.Instantiate<Stage2D>();
		}
		else
		{
			if (meta.Stage == fallBackStage)
				throw new Exception($"Fallback stage \"{fallBackStage}\" was not a PackedScene.");

			PrintUtility.PrintError("CanvasItemSpace", $"Stage \"{meta.Stage}\" is not a PackedScene. Falling back to default.");
			meta.Stage = fallBackStage;
			Initialize(meta);
			return;
		}
		
		if (Stage == null)
		{
			if (meta.Stage == fallBackStage)
				throw new Exception($"Fallback stage \"{fallBackStage}\" failed to instantiate.");
			
			PrintUtility.PrintError("CanvasItemSpace", $"Stage \"{meta.Stage}\" failed to instantiate. Falling back to default.");
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
		Camera.ZoomMotionData.LerpWeight = 3.125f;
		
		// Init characters
		Characters = [];
		_namedCharacters = new Dictionary<StringName, Character2D>();
		_characterGroups = new Dictionary<StringName, CharacterGroup2D>();
		_characterScenes = new Dictionary<string, PackedScene>();
		for (int i = 0; i < meta.Characters.Length; i++)
			AddCharacter(meta.Characters[i]);
		
		Initialized = true;
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
			PrintUtility.PrintError("CanvasItemSpace", $"Character \"{meta.Character}\" was not found. Falling back to default.");
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
					PrintUtility.PrintError("CanvasItemSpace", $"Character \"{meta.Character}\" is a 3D character. Falling back to default.");
					AddFallbackCharacter(meta);
					return;
				}
				
				_characterScenes.Add(meta.Character, packedScene);
				character = packedScene.Instantiate<Character2D>();
			}
			else
			{
				PrintUtility.PrintError("CanvasItemSpace", $"Character \"{meta.Character}\" is not inside a PackedScene. Falling back to default.");
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
		PrintUtility.Print("CanvasItemSpace", $"Added Character: {meta.Character}", true);
	}

	public Character2D GetCharacter(StringName nickName) => _namedCharacters[nickName];
	
	public CharacterGroup2D GetCharacterGroup(StringName groupName) => _characterGroups[groupName];

	private void AddFallbackCharacter(CharacterMeta meta)
	{
		string fallBackCharacter = ProjectSettings.GetSetting("rubicon/general/fallback/character_2d").AsString();
		string fallBackPath = PathUtility.GetScenePath($"res://Resources/Game/Characters/{fallBackCharacter}");
		if (!ResourceLoader.Exists(fallBackPath)) // Bro
		{
			PrintUtility.PrintError("CanvasItemSpace", "No character fallback was found. Please check your project settings at \"rubicon/general/fallback/character\". Skipping.");
			return;
		}
			
		meta.Character = fallBackCharacter;
		AddCharacter(meta);
	}
}
