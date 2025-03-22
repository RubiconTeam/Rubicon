using System.Collections.Generic;
using PukiTools.GodotSharp;
using PukiTools.GodotSharp.Audio;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Rubicon.Core.Data;
using Rubicon.Core.Events;
using Rubicon.Core.Rulesets;
using Rubicon.View2D;
using Rubicon.View3D;

namespace Rubicon.Game;

/// <summary>
/// The main node that brings characters, stages, and ruleset gameplay together. Serves as "PlayState" in other Funkin' engines.
/// </summary>
[GlobalClass, Autoload("RubiconGame")]
public partial class RubiconGameInstance : CanvasLayer
{
	/// <summary>
	/// Disables <see cref="_Process"/> and <see cref="_Input"/> when false.
	/// </summary>
	[Export] public bool Active = false;
	
	/// <summary>
	/// The load context of the current song.
	/// Helps loading songs, charts, metadata and others.
	/// </summary>
	[Export] public LoadContext Context;
	
	/// <summary>
	/// The instrumental of the current song and difficulty.
	/// Obligatory for the song to work.
	/// </summary>
	[Export] public AudioStreamPlayer Instrumental
	{
		get => PlayField.Music;
		set => PlayField.Music = value;
	}

	/// <summary>
	/// The vocals of the current song.
	/// Not required for the song to start.
	/// </summary>
	[Export] public AudioStreamPlayer Vocals;
	
	[ExportGroup("Status"), Export] public bool Paused = false;

	/// <summary>
	/// The metadata of the current song.
	/// Obligatory for the song to work.
	/// </summary>
	[Export] public SongMeta Metadata;

	/// <summary>
	/// The chart of the current song.
	/// Obligatory for the song to work.
	/// </summary>
	[Export] public RubiChart Chart;

	/// <summary>
	/// The events of the current song.
	/// Not required for the song to start.
	/// </summary>
	[Export] public EventMeta Events;
	
	/// <summary>
	/// The RuleSet of the current song.
	/// The default RuleSet is Mania, found in Project Settings (rubicon/rulesets/default_ruleset).
	/// </summary>
	[ExportGroup("References"), Export] public RuleSet RuleSet;
	
	/// <summary>
	/// The PlayField of the current song.
	/// </summary>
	[Export] public PlayField PlayField;

	/// <summary>
	/// Executes <see cref="Bounce"/> depending on the bpm,
	/// <see cref="TimeValue"/> and step.
	/// </summary>
	[Export] public BeatSyncer BounceBeatSyncer;

	/// <summary>
	/// The Root/Parent node of <see cref="RubiconGameInstance"/>, usually <see cref="RubiconGameScreen"/>
	/// </summary>
	[Export] public Node RootNode;

	/// <summary>
	/// The environment responsible for handling 2D-related nodes (i.e. 2D characters and stages)
	/// </summary>
	[Export] public CanvasItemSpace CanvasItemSpace;
	
	/// <summary>
	/// The environment responsible for handling 3D-related nodes (i.e. 3D characters and stages)
	/// </summary>
	[Export] public SpatialSpace SpatialSpace;

	/// <summary>
	/// The class responsible for loading and executing events.
	/// </summary>
	[Export] public SongEventController EventController;

	private string[] _actionNames;

	public override void _Ready()
	{
		base._Ready();

		EventController = new SongEventController();
		AddChild(EventController);
	}

	/// <summary>
	/// Sets up <see cref="RubiconGameInstance"/> for use.
	/// Loads RuleSets, Charts and Metadata, PlayField 
	/// </summary>
	/// <param name="rootNode"></param>
	/// <exception cref="Exception"></exception>
	public void Setup(Node rootNode)
	{
		if (!Context.IsValid())
			return;

		Layer = 16;
		
		RootNode = rootNode ?? this;
		
		// Set up rule set, and check paths too
		string ruleSetName = Context.RuleSet;
		RuleSet = LoadRuleSet(ruleSetName);
		if (RuleSet is null)
		{
			string fallbackName = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
			PrintUtility.PrintError("RubiconGame", $"Rule set \"{ruleSetName}\" was not able to be loaded. Falling back to \"{fallbackName}\".");
			RuleSet = LoadRuleSet(fallbackName);
		}

		if (RuleSet is null) // You fucked up again bro
			throw new Exception("RuleSet is still null. Please check your Project Settings at \"rubicon/rulesets\"");
		PrintUtility.Print("RubiconGame", $"Rule Set \"{RuleSet.Name}\" loaded successfully.", true);
		
		if (Metadata.Vocals is not null)
		{
			Vocals = AudioManager.GetGroup("Music").Play(Metadata.Vocals, false);
			PrintUtility.Print("RubiconGame", $"Vocals found and loaded", true);
		}

		LoadSpace(Metadata);
		
		// Set up play field
		PlayField = LoadPlayField(RuleSet);
		PlayField.Setup(Metadata, Chart, Context.TargetIndex, Events);
		PlayField.NoteHit += NoteHit;
		AddChild(PlayField);
		PrintUtility.Print("RubiconGame", "PlayField loaded successfully.", true);
		
		BarLine targetBarLine = PlayField.BarLines[PlayField.TargetIndex];
		_actionNames = new string[targetBarLine.Managers.Length];
		for (int i = 0; i < _actionNames.Length; i++)
			_actionNames[i] = targetBarLine.Managers[i].Action;
		
		BounceBeatSyncer = new BeatSyncer();
		BounceBeatSyncer.Name = "UI Bumper";
		BounceBeatSyncer.Value = 1f;
		BounceBeatSyncer.Bumped += Bounce;
		AddChild(BounceBeatSyncer);
		
		LoadGameScripts();

		/*
		float SPEED = 1f;
		PlayField.Music.PitchScale = SPEED;
		Vocals?.SetPitchScale(SPEED);
		Conductor.Speed = SPEED;*/
		
		// TODO: Countdown
		PlayField.Start();
		Vocals?.Play();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!Active)
			return;
		
		// TODO: Optimize this later
		Vector2 playFieldScale = PlayField.Scale;
		PlayField.Scale = playFieldScale.Lerp(Vector2.One, 3.125f * (float)delta);

		PlayField.PivotOffset = PlayField.Size / 2f;
	}

	/// <summary>
	/// Handle camera bouncing set by <see cref="BounceBeatSyncer"/>.
	/// </summary>
	public void Bounce()
	{
		PlayField.Scale += Vector2.One * 0.015f;
		switch (Metadata.Environment)
		{
			case GameEnvironment.CanvasItem:
				CanvasItemSpace.Camera.Zoom += Vector2.One * 0.045f;
				break;
			case GameEnvironment.Spatial:
				SpatialSpace.Camera.Fov -= 2;
				break;
		}
	}

	/// <summary>
	/// Gets called every time a note has been hit.
	/// Handles singing and missing animations for each environment.
	/// </summary>
	/// <param name="name">The name for the note type of the hit note.</param>
	/// <param name="result">The result of the hit note.</param>
	protected virtual void NoteHit(StringName name, NoteResult result)
	{
		if (result.Rating == Judgment.None)
			return;
		
		bool missed = result.Rating == Judgment.Miss;
		if (!result.Flags.HasFlag(NoteResultFlags.Animation))
		{
			switch (Metadata.Environment)
			{
				case GameEnvironment.CanvasItem: // 2D Space
				{
					CharacterGroup2D characterGroup = CanvasItemSpace.GetCharacterGroup(name);
					characterGroup.Sing(result.Direction, !missed && result.Hit == Hit.Hold, missed);
					
					// TODO: Bad code, need to consider other characters
					bool charactersMissed = false;
					for (int i = 0; i < characterGroup.Characters.Count; i++)
						if (characterGroup.Characters[i].Missed)
							charactersMissed = true;

					missed = charactersMissed;
					break;
				}
				case GameEnvironment.Spatial: // 3D Space
				{
					CharacterGroup3D characterGroup = SpatialSpace.GetCharacterGroup(name);
					characterGroup.Sing(result.Direction, !missed && result.Hit == Hit.Hold, missed);
					
					// TODO: Bad code, need to consider other characters
					bool charactersMissed = false;
					for (int i = 0; i < characterGroup.Characters.Count; i++)
						if (characterGroup.Characters[i].Missed)
							charactersMissed = true;

					missed = charactersMissed;
					break;
				}
			}
		}

		if (!result.Flags.HasFlag(NoteResultFlags.Vocals) && Vocals is not null)
			Vocals.VolumeLinear = missed ? 0f : 1f;
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		// Temporary
		if (@event.IsActionPressed("game_pause"))
		{
			if (!Paused)
				Pause();
			else
				Resume();
		}

		if (!Active || @event.IsEcho())
			return;

		// Freeze singing
		bool isAction = false;
		bool isHolding = false;
		for (int i = 0; i < _actionNames.Length; i++)
		{
			if (@event.IsAction(_actionNames[i]))
			{
				if (@event.IsPressed())
					isHolding = true;
				
				isAction = true;	
			}
			
			if (Input.IsActionPressed(_actionNames[i]))
				isHolding = true;
		}

		if (!isAction)
			return;

		switch (Metadata.Environment)
		{
			case GameEnvironment.CanvasItem:
				CanvasItemSpace.GetCharacterGroup(PlayField.TargetBarLine).SetFreezeSinging(isHolding);
				break;
			case GameEnvironment.Spatial:
				SpatialSpace.GetCharacterGroup(PlayField.TargetBarLine).SetFreezeSinging(isHolding);
				break;
		}
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void Pause()
	{
		Paused = true;
		
		PlayField.Pause();
		Vocals?.Stop();
	}

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public void Resume()
	{
		Paused = false;
		
		PlayField.Resume();
		Vocals?.Play(Conductor.RawTime);
	}

	/// <summary>
	/// Resets the song as well as all of its components.
	/// </summary>
	public void Reset()
	{
		Active = false;
			
		AudioPlayerGroup musicGroup = AudioManager.GetGroup("Music");
		musicGroup.Stop();
		musicGroup.DestroyPlayer(Instrumental);
		musicGroup.DestroyPlayer(Vocals);
		
		PlayField.QueueFree();
		BounceBeatSyncer.QueueFree();
		
		RuleSet = null;
		RootNode = null;

		switch (Metadata.Environment)
		{
			case GameEnvironment.CanvasItem:
				CanvasItemSpace.QueueFree();
				break;
			case GameEnvironment.Spatial:
				SpatialSpace.QueueFree();
				break;
		}	

		Metadata = null;
		Chart = null;
		Events = null;

		EventController.Reset();
	}
	
	/// <summary>
	/// Loads the ruleset provided via the ruleSetName parameter.
	/// In case of being null, a fallback found in ProjectSettings will be loaded.
	/// </summary>
	/// <param name="ruleSetName">The name of the <see cref="RuleSet"/></param>
	/// <returns>A valid <see cref="RuleSet"/></returns>
	private RuleSet LoadRuleSet(string ruleSetName)
	{
		string ruleSetResourcePath = PathUtility.GetResourcePath($"res://resources/game/rulesets/{ruleSetName}");
		if (string.IsNullOrWhiteSpace(ruleSetResourcePath))
		{
			PrintUtility.PrintError("RubiconGame", $"No resource exists at path \"{ruleSetResourcePath}\".");
			return null;
		}

		Variant ruleSetResource = ResourceLoader.LoadThreadedGet(ruleSetResourcePath);
		return ruleSetResource.As<RuleSet>();
	}

	private PlayField LoadPlayField(RuleSet ruleSet)
	{
		/*
		Script script = ruleSet.PlayFieldScript;
		if (script is not CSharpScript cSharpScript)
		{
			GD.PrintErr($"Rulesets do not support other languages other than C# at the moment. (path: \"{ruleSet.ResourcePath}\")");
			return null;
		}*/

		GodotObject ruleSetObject = ruleSet.PlayFieldScript.New().AsGodotObject();
		if (ruleSetObject is not PlayField playField)
		{
			PrintUtility.PrintError("RubiconGame", $"Ruleset at path \"{ruleSet.ResourcePath}\" does not contain a valid PlayField script.");
			return null;
		}

		return playField;
	}

	/// <summary>
	/// Loads each <see cref="GameEnvironment"/> node and initializes it.
	/// </summary>
	/// <param name="songMeta">The metadata of the song.</param>
	private void LoadSpace(SongMeta songMeta)
	{
		PrintUtility.Print("RubiconGame", "Environment: " + songMeta.Environment, true);
		switch (songMeta.Environment)
		{
			case GameEnvironment.None:
				Active = true;
				break;
			case GameEnvironment.CanvasItem: // 2D Space
			{
				CanvasItemSpace = new CanvasItemSpace();
				CanvasItemSpace.Name = "Space";
				CanvasItemSpace.Initialize(songMeta);
				// Check if CanvasItemSpace was initialized to
				// not fill the debugger with NullReferenceExceptions
				Active = CanvasItemSpace.Initialized;
				
				// This is mostly a fallback for any uncovered exception when loading spaces.
				// If anything fails and it is not stopped by an exception, you'll end up here
				if (!CanvasItemSpace.Initialized)
				{
					Active = true;
					songMeta.Environment = GameEnvironment.None; 
					GD.PushError("An error was found initializing CanvasItemSpace, this does not relate to character or stage loading. Continuing without GameEnvironment");
					return;
				}
				
				RootNode.AddChild(CanvasItemSpace);
				break;
			}
			case GameEnvironment.Spatial:
				SpatialSpace = new SpatialSpace();
				SpatialSpace.Name = "Space";
				SpatialSpace.Initialize(songMeta);
				// Check if SpatialSpace was initialized to
				// not fill the debugger with NullReferenceExceptions
				Active = SpatialSpace.Initialized;

				// This is mostly a fallback for any uncovered exception when loading spaces.
				// If anything fails and it is not stopped by an exception, you'll end up here
				if (!SpatialSpace.Initialized)
				{
					Active = true;
					songMeta.Environment = GameEnvironment.None; 
					GD.PushError("An error was found initializing SpatialSpace. Continuing without GameEnvironment");
					return;
				}

				RootNode.AddChild(SpatialSpace);
				break;
		}
		
		PrintUtility.Print("RubiconGame", "Space created successfully.", true);
	}

	/// <summary>
	/// Loads each song script, as well as global/common scripts.
	/// </summary>
	private void LoadGameScripts()
	{
		List<string> scriptPaths = [];
		scriptPaths.AddRange(PathUtility.GetAbsoluteFilePathsAt("res://resources/game/common/", true));
		scriptPaths.AddRange(PathUtility.GetAbsoluteFilePathsAt($"res://songs/{Context.Name}/scripts/", true));
		for (int i = 0; i < scriptPaths.Count; i++)
		{
			string path = scriptPaths[i];
			string ext = path.GetExtension().ToLower();
			bool isScene = ext == "tscn" || ext == "scn";
			bool isGdScript = ext == "gd";
			bool isCSharpScript = ext == "cs";
			if (!isScene && !isGdScript && !isCSharpScript)
				continue;

			Resource resource = ResourceLoader.LoadThreadedGet(path);
			if (isScene && resource is PackedScene packedScene)
			{
				AddChild(packedScene.Instantiate());
				continue;
			}

			if (isGdScript && resource is GDScript gdScript)
			{
				AddChild(gdScript.New().As<Node>());
				continue;
			}

			if (isCSharpScript && resource is CSharpScript cSharpScript)
			{
				AddChild(cSharpScript.New().As<Node>());
				continue;
			}
		}
	}
}
