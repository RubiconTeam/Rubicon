using Godot.Collections;
using Rubicon.Core.Meta;

namespace Rubicon.View2D;

[GlobalClass]
public partial class CanvasItemSpace : Node2D
{
	[Export] public RubiconCamera2D Camera;

	[Export] public Array<Character2D> Characters;

	[Export] public Stage2D Stage;

	private Dictionary<StringName, Character2D> _namedCharacters;
	private Dictionary<StringName, Array<Character2D>> _spawnPointCharacters;
	private Dictionary<StringName, Array<Character2D>> _barLineCharacters;
	
	public void Initialize(SongMeta meta)
	{
		// Init stage
		Stage = GD.Load<PackedScene>($"res://Resources/Stages/{meta.Stage}.tscn").Instantiate<Stage2D>(); // TODO: Check if stage even exists
		AddChild(Stage);
		
		// Init characters
		Characters = new Array<Character2D>();
		_namedCharacters = new Dictionary<StringName, Character2D>();
		_spawnPointCharacters = new Dictionary<StringName, Array<Character2D>>();
		_barLineCharacters = new Dictionary<StringName, Array<Character2D>>();
		for (int i = 0; i < meta.Characters.Length; i++)
			AddCharacter(meta.Characters[i]);
	}

	public void AddCharacter(CharacterMeta meta)
	{
		string path = $"res://Resources/Characters/{meta.Character}.tscn";
		Character2D character;
			
		if (!ResourceLoader.Exists(path))
		{
			GD.Print($"Character {meta.Character} was not found. Falling back to default.");
			string fallBack = $"res://Resources/Characters/{ProjectSettings.GetSetting("rubicon/general/fallback/character").AsString()}.tscn";
			if (!ResourceLoader.Exists(fallBack)) // Bro
			{
				GD.PrintErr("No character fallback was found. Please check your project settings at \"rubicon/general/fallback/character\". Skipping.");
				return;
			}

			character = ResourceLoader.Load<PackedScene>(fallBack).Instantiate<Character2D>();
		}
		else
		{
			character = ResourceLoader.Load<PackedScene>(path).Instantiate<Character2D>();
		}
		
		Characters.Add(character);
		_namedCharacters[meta.Nickname] = character;
		
		if (!_spawnPointCharacters.ContainsKey(meta.SpawnPoint))
			_spawnPointCharacters.Add(meta.SpawnPoint, new Array<Character2D>());
		
		_spawnPointCharacters[meta.SpawnPoint].Add(character);
		Stage.SpawnPoints[meta.SpawnPoint].AddChild(character);
		
		if (!_barLineCharacters.ContainsKey(meta.BarLine))
			_barLineCharacters.Add(meta.BarLine, new Array<Character2D>());
		
		_barLineCharacters[meta.BarLine].Add(character);
	}
}
