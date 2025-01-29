using System.Collections.Generic;
using System.Linq;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Meta;
using Rubicon.Core.Data;
using Rubicon.Core.Rulesets;
using Rubicon.Screens;
using Rubicon.View2D;

namespace Rubicon.Game;

/// <summary>
/// The main node that brings characters, stages, and ruleset gameplay together. Serves as "PlayState" in other Funkin' engines.
/// </summary>
[GlobalClass] public partial class RubiconGame : CsScreen
{
	public static RubiconGame Instance { get; private set; }

	public static LoadContext Context;

	#if TOOLS
	[Export] public string EditorSongName = ProjectSettings.GetSetting("rubicon/general/fallback/song").AsString();
	
	[Export] public string EditorDifficulty = ProjectSettings.GetSetting("rubicon/general/fallback/difficulty").AsString();
	
	[Export] public string EditorRuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
	#endif
	
	[ExportGroup("Status"), Export] public bool Paused = false;

	[Export] public SongMeta Metadata;

	[Export] public RubiChart Chart;
	
	[ExportGroup("References"), Export] public RuleSet RuleSet;
	
	[Export] public PlayField PlayField;

	[Export] public CanvasItemSpace CanvasItemSpace;

	[Export] public SongEventController EventController;

	[ExportSubgroup("Audio"), Export] public AudioStreamPlayer Instrumental;
	
	[Export] public AudioStreamPlayer Vocals;

	private bool _preloaded = false;

	public override void ReadyPreload()
	{
		_preloaded = true;
		
		// Just load the song meta for now, we'll wait until it's loaded.
		List<string> resourceList = ResourcesToLoad.ToList();
		resourceList.Add($"res://Songs/{Context.Name}/Data/Meta.tres");
		resourceList.Add($"res://Songs/{Context.Name}/Data/{Context.RuleSet}-{Context.Difficulty}.rbc");
		resourceList.Add($"res://Songs/{Context.Name}/Inst.ogg");

		string vocalsPath = $"res://Songs/{Context.Name}/Vocals.ogg";
		if (ResourceLoader.Exists(vocalsPath))
			resourceList.Add(vocalsPath);
		
		ResourcesToLoad = resourceList.ToArray();
	}

	public override void OnPreload(string path)
	{
		string metaPath = $"res://Songs/{Context.Name}/Data/Meta.tres";
		if (path == metaPath)
		{
			Metadata = ResourceLoader.LoadThreadedGet(metaPath) as SongMeta;
			List<string> resourceList = ResourcesToLoad.ToList();
			resourceList.Add($"res://Resources/UI/Styles/{Metadata.UiStyle}/Style.tres");
			resourceList.Add($"res://Resources/UI/Styles/{Metadata.NoteSkin}/{Context.RuleSet}.tres");
			resourceList.Add($"res://Resources/Rulesets/{Context.RuleSet}.tres");
			resourceList.Add($"res://Resources/Stages/{Metadata.Stage}.tscn");

			List<string> loadedCharacters = new List<string>();
			for (int i = 0; i < Metadata.Characters.Length; i++)
			{
				string curCharacter = Metadata.Characters[i].Character;
				if (loadedCharacters.Contains(curCharacter))
					continue;
				
				resourceList.Add($"res://Resources/Characters/{curCharacter}.tscn");
				loadedCharacters.Add(curCharacter);
			}
				
			ResourcesToLoad = resourceList.ToArray();
		}
	}

	public override void _Ready()
	{
		#if TOOLS
		if (Context == null)
			Context = new LoadContext { Name = EditorSongName, Difficulty = EditorDifficulty, RuleSet = EditorRuleSet };
		#endif

		if (!_preloaded)
		{
			ScreenManager.SwitchScreen("res://Screens/RubiconGame.tscn", "default");
			return;
		}

		if (!Context.IsValid())
			return;
		
		if (Instance != null)
		{
			QueueFree();
			return;
		}
		
		Instance = this;
		
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
		
		string instPath = $"res://Songs/{Context.Name}/Inst.ogg";
		string vocalsPath = $"res://Songs/{Context.Name}/Vocals.ogg";
		if (ResourceLoader.Exists(instPath))
			Instrumental.Stream = ResourceLoader.LoadThreadedGet(instPath) as AudioStream;
		else
			GD.PrintErr($"Audio file at path \"{instPath}\" was not found!");

		if (ResourceLoader.Exists(vocalsPath))
			Vocals.Stream = ResourceLoader.LoadThreadedGet(vocalsPath) as AudioStream;

		Conductor.Reset();
		Conductor.ChartOffset = Metadata.Offset;
		Conductor.BpmList = Metadata.BpmInfo;
		
		// TODO: Make Spaces work
		LoadSpace();
		
		// Set up play field
		PlayField = LoadPlayField(RuleSet);
		PlayField.Setup(Metadata, Chart, Context.TargetIndex);
		PlayField.SingCalled += SingCalled;
		AddChild(PlayField);

		LoadAutoLoads();
		
		// TODO: Countdown
		Conductor.Play();
		
		if (Instrumental.Stream != null)
			Instrumental.Play();
		
		if (Vocals.Stream != null)
			Vocals.Play();
	}

	protected virtual void SingCalled(StringName name, NoteResult result)
	{
		switch (Metadata.Environment)
		{
			case GameEnvironment.CanvasItem: // 2D Space
			{
				CanvasItemSpace.SingGroup(name, result.Direction, result.Holding, result.Hit == HitType.Miss);
				break;
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsActionPressed("game_pause"))
		{
			if (!Paused)
				Pause();
			else
				Resume();
		}
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void Pause()
	{
		Conductor.Pause();	
		PlayField.ProcessMode = ProcessModeEnum.Disabled;
		Paused = true;
		
		if (Instrumental.Stream != null)
			Instrumental.Stop();
		
		if (Vocals.Stream != null)
			Vocals.Stop();
	}

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public void Resume()
	{
		Conductor.Resume();	
		PlayField.ProcessMode = ProcessModeEnum.Inherit;
		Paused = false;

		float time = (float)Conductor.RawTime;
		if (Instrumental.Stream != null)
			Instrumental.Play(time);

		if (Vocals.Stream != null)
			Vocals.Play(time);
	}
	
	private RuleSet LoadRuleSet(string ruleSetName)
	{
		string ruleSetResourcePath = $"res://Resources/Rulesets/{ruleSetName}.tres";
		if (!ResourceLoader.Exists(ruleSetResourcePath))
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

	private void LoadSpace()
	{
		GD.Print("Environment: " + Metadata.Environment);
		switch (Metadata.Environment)
		{
			case GameEnvironment.CanvasItem: // 2D Space
			{
				CanvasItemSpace = new CanvasItemSpace();
				CanvasItemSpace.Name = "Space";
				CanvasItemSpace.Initialize(Metadata);
				AddChild(CanvasItemSpace);
				break;
			}
		}
		
		GD.Print("Space created successfully.");
	}

	private void LoadAutoLoads()
	{
		string autoLoadDir = "res://Resources/Autoloads/";
		string[] autoLoadPaths = RubiconUtility.GetFilesAt(autoLoadDir, true);
		for (int i = 0; i < autoLoadPaths.Length; i++)
		{
			Resource autoLoadRes = GD.Load(autoLoadDir + autoLoadPaths[i]);
			if (autoLoadRes is Script autoLoadScript)
			{
				Node node = new Node();
				node.SetScript(autoLoadScript);
				AddChild(node);
			}

			if (autoLoadRes is PackedScene autoLoadScene)
			{
				Node scene = autoLoadScene.Instantiate();
				AddChild(scene);
			}
		}
		
		GD.Print("Loading song auto-loads successful.");
	}
}
