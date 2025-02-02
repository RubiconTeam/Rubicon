using System.Collections.Generic;
using System.Data;
using System.Linq;
using Rubicon.Core;
using Rubicon.Core.Audio;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Rubicon.Core.Data;
using Rubicon.Core.Events;
using Rubicon.Core.Rulesets;
using Rubicon.Data;
using Rubicon.Screens;
using Rubicon.View2D;

namespace Rubicon.Game;

/// <summary>
/// The main node that brings characters, stages, and ruleset gameplay together. Serves as "PlayState" in other Funkin' engines.
/// </summary>
[GlobalClass, StaticAutoloadSingleton("Rubicon.Game", "RubiconGame")]
public partial class RubiconGameInstance : CanvasLayer
{
	[Export] public bool Active = false;
	
	[Export] public LoadContext Context;
	
	[Export] public AudioStreamPlayer Instrumental
	{
		get => PlayField.Music;
		set => PlayField.Music = value;
	}

	[Export] public AudioStreamPlayer Vocals;
	
	[ExportGroup("Status"), Export] public bool Paused = false;

	[Export] public SongMeta Metadata;

	[Export] public RubiChart Chart;

	[Export] public EventMeta Events;
	
	[ExportGroup("References"), Export] public RuleSet RuleSet;
	
	[Export] public PlayField PlayField;

	[Export] public Bumper BounceBumper;

	[Export] public Node RootNode;

	[Export] public CanvasItemSpace CanvasItemSpace;

	[Export] public SongEventController EventController;

	private string[] _actionNames;

	public override void _Ready()
	{
		base._Ready();

		EventController = new SongEventController();
		AddChild(EventController);
	}

	public void Setup(Node rootNode)
	{
		if (!Context.IsValid())
			return;

		Layer = 16;
		
		RootNode = rootNode ?? this;
		Active = true;
		
		// Set up rule set, and check paths too
		string ruleSetName = Context.RuleSet;
		RuleSet = LoadRuleSet(ruleSetName);
		if (RuleSet is null)
		{
			string fallbackName = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
			GD.PrintErr($"Rule set \"{ruleSetName}\" was not able to be loaded. Falling back to \"{fallbackName}\".");
			RuleSet = LoadRuleSet(fallbackName);
		}

		if (RuleSet is null) // You fucked up again bro
			throw new Exception("RuleSet is still null. Please check your Project Settings at \"rubicon/rulesets\"");

		string chartPath = $"res://Songs/{Context.Name}/Data/{Context.RuleSet}-{Context.Difficulty}.rbc";
		Chart = ResourceLoader.LoadThreadedGet(chartPath) as RubiChart;
		
		if (Metadata.Vocals is not null)
			Vocals = AudioManager.Music.AddSubTrack(Metadata.Vocals, false, false);	
			
		LoadSpace(Metadata);
		
		// Set up play field
		PlayField = LoadPlayField(RuleSet);
		PlayField.Setup(Metadata, Chart, Context.TargetIndex, Events);
		PlayField.NoteHit += NoteHit;
		AddChild(PlayField);
		
		BarLine targetBarLine = PlayField.BarLines[PlayField.TargetIndex];
		_actionNames = new string[targetBarLine.Managers.Length];
		for (int i = 0; i < _actionNames.Length; i++)
			_actionNames[i] = targetBarLine.Managers[i].Action;
		
		BounceBumper = new Bumper();
		BounceBumper.Name = "UI Bumper";
		BounceBumper.BumpMeasure = 1f;
		BounceBumper.Bumped += Bounce;
		AddChild(BounceBumper);
		
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

	public void Bounce()
	{
		PlayField.Scale += Vector2.One * 0.015f;
		switch (Metadata.Environment)
		{
			case GameEnvironment.CanvasItem:
				CanvasItemSpace.Camera.Zoom += Vector2.One * 0.045f;
				break;
		}
	}

	protected virtual void NoteHit(StringName name, NoteResult result)
	{
		if (result.Flags.HasFlag(NoteResultFlags.Animation))
			return;

		bool missed = result.Hit == HitType.Miss;
		switch (Metadata.Environment)
		{
			case GameEnvironment.CanvasItem: // 2D Space
			{
				CanvasItemSpace.SingForGroup(name, result.Direction, !missed && result.Holding, missed);
				break;
			}
		}
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

		if (@event.IsEcho())
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
				CanvasItemSpace.FreezeSingingForGroup(PlayField.TargetBarLine, isHolding);
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

	public void Reset()
	{
		Active = false;
		AudioManager.Music.Stop();
		AudioManager.Music.RemoveSubPlayer(Vocals);
		
		PlayField.QueueFree();
		BounceBumper.QueueFree();
		
		RuleSet = null;
		RootNode = null;

		switch (Metadata.Environment)
		{
			case GameEnvironment.CanvasItem:
				CanvasItemSpace.QueueFree();
				break;
		}	

		Metadata = null;
		Chart = null;
		Events = null;

		EventController.Reset();
	}
	
	private RuleSet LoadRuleSet(string ruleSetName)
	{
		string ruleSetResourcePath = PathUtility.GetResourcePath($"res://Resources/Rulesets/{ruleSetName}");
		if (string.IsNullOrWhiteSpace(ruleSetResourcePath))
		{
			GD.PrintErr($"No resource exists at path \"{ruleSetResourcePath}\".");
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
			GD.PrintErr($"Ruleset at path \"{ruleSet.ResourcePath}\" does not contain a valid PlayField script.");
			return null;
		}

		return playField;
	}

	private void LoadSpace(SongMeta songMeta)
	{
		GD.Print("Environment: " + songMeta.Environment);
		switch (songMeta.Environment)
		{
			case GameEnvironment.CanvasItem: // 2D Space
			{
				CanvasItemSpace = new CanvasItemSpace();
				CanvasItemSpace.Name = "Space";
				CanvasItemSpace.Initialize(songMeta);
				RootNode.AddChild(CanvasItemSpace);
				break;
			}
		}
		
		GD.Print("Space created successfully.");
	}
}
