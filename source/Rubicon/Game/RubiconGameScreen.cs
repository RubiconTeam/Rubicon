using System.Collections.Generic;
using PukiTools.GodotSharp.Screens;
using Rubicon.Core;
using Rubicon.Core.Chart;
using Rubicon.Core.Events;
using Rubicon.Core.Meta;

namespace Rubicon.Game;

[GlobalClass] public partial class RubiconGameScreen : CsScreen
{
#if TOOLS
	[Export] public string SongName = ProjectSettings.GetSetting("rubicon/general/fallback/song").AsString();
	
	[Export] public string Difficulty = ProjectSettings.GetSetting("rubicon/general/fallback/difficulty").AsString();
	
	[Export] public string RuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
#endif
    
    public override void ReadyPreload()
	{
		base.ReadyPreload();
		
		// Just load the song meta for now, we'll wait until it's loaded.
		LoadContext context = RubiconGame.Context;
		ResourcesToLoad.AddResource($"res://songs/{context.Name}/data/Meta");
		ResourcesToLoad.AddPath($"res://songs/{context.Name}/data/{context.RuleSet}-{context.Difficulty}.rbc");

		string eventsPath = $"res://songs/{context.Name}/data/Events";
		if (PathUtility.ResourceExists(eventsPath))
			ResourcesToLoad.AddResource($"res://songs/{context.Name}/data/Events");

		List<string> scriptPaths = [];
		scriptPaths.AddRange(PathUtility.GetAbsoluteFilePathsAt("res://resources/game/common/", true));
		scriptPaths.AddRange(PathUtility.GetAbsoluteFilePathsAt($"res://songs/{context.Name}/scripts/", true));
		for (int i = 0; i < scriptPaths.Count; i++)
		{
			string path = scriptPaths[i];
			string ext = path.GetExtension().ToLower();
			if (ext != "tscn" && ext != "scn" && ext != "cs" && ext != "gd")
				continue;
			
			ResourcesToLoad.AddPath(path);
		}
	}

	public override void OnPreload(string path)
	{
		LoadContext context = RubiconGame.Context;
		string metaPath = ResourcesToLoad.GetResourcePath($"res://songs/{context.Name}/data/Meta");
		if (path == metaPath) // Meta loaded
		{
			Resource metaResource = ResourceLoader.LoadThreadedGet(metaPath);
			if (metaResource is not SongMeta meta)
				return;

			RubiconGame.Metadata = meta;
			
			// Chart
			RubiChart chart = meta.GetDifficultyByName(context.Difficulty, context.RuleSet).Chart;
			List<string> noteTypes = new List<string>();
			RubiconGame.Chart = chart;
			for (int i = 0; i < chart.Charts.Length; i++)
				for (int n = 0; n < chart.Charts[i].Notes.Length; n++)
					if (chart.Charts[i].Notes[n].Type != "Normal" && !noteTypes.Contains(chart.Charts[i].Notes[n].Type))
						noteTypes.Add(chart.Charts[i].Notes[n].Type);
			
			for (int i = 0; i < noteTypes.Count; i++)
				if (RubiconCore.NoteTypePaths.ContainsKey(noteTypes[i]))
					ResourcesToLoad.AddPath(RubiconCore.NoteTypePaths[noteTypes[i]]);
			
			for (int i = 0; i < noteTypes.Count; i++)
			{	
				string noteTypePath = $"res://resources/game/notetypes/{noteTypes[i]}";
				if (ResourceLoader.Exists(noteTypePath + ".tscn") || ResourceLoader.Exists(noteTypePath + ".scn"))
					ResourcesToLoad.AddScene(noteTypePath);
				
				if (ResourceLoader.Exists(noteTypePath + ".gd"))
					ResourcesToLoad.AddPath(noteTypePath + ".gd");
				
				if (ResourceLoader.Exists(noteTypePath + ".cs"))
					ResourcesToLoad.AddPath(noteTypePath + ".cs");
			}
			
			string uiStylePath = $"res://resources/ui/styles/{meta.UiStyle}/Style";
			if (!PathUtility.ResourceExists(uiStylePath))
				uiStylePath = $"res://resources/ui/styles/{ProjectSettings.GetSetting("rubicon/general/default_ui_style")}/Style";
				
			ResourcesToLoad.AddResource(uiStylePath);
			
			string ruleSetLoadPath = $"res://resources/game/rulesets/{context.RuleSet}";
			if (!PathUtility.ResourceExists(ruleSetLoadPath))
			{
				context.RuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
				ruleSetLoadPath = $"res://resources/game/rulesets/{context.RuleSet}";
			}
			
			ResourcesToLoad.AddResource(ruleSetLoadPath);
			
			if (meta.Environment == GameEnvironment.None)
				return;
			
			string envSuffix = meta.Environment == GameEnvironment.CanvasItem ? "2d" : "3d";
			string stagePath = $"res://resources/game/stages/{meta.Stage}";
			if (!PathUtility.SceneExists(stagePath)) // Stage Fallback
			{
				string fallBackStage = ProjectSettings.GetSetting("rubicon/general/fallback/stage_" + envSuffix).AsString();
				stagePath = $"res://resources/game/stages/{fallBackStage}";
				if (!PathUtility.SceneExists(stagePath)) // Dude
					return;
			}
			
			ResourcesToLoad.AddScene(stagePath);
			
			List<string> loadedCharacters = [];
			for (int i = 0; i < meta.Characters.Length; i++)
			{
				string curCharacter = meta.Characters[i].Character;
				if (loadedCharacters.Contains(curCharacter))
					continue;

				string charaPath = $"res://resources/game/characters/{curCharacter}";
				if (!PathUtility.SceneExists(charaPath)) // Fallback
				{
					curCharacter = ProjectSettings.GetSetting("rubicon/general/fallback/character_" + envSuffix).AsString();	
					charaPath = $"res://resources/game/characters/{curCharacter}";
					
					if (!PathUtility.SceneExists(charaPath))
						continue;
				}
				
				if (loadedCharacters.Contains(curCharacter))
					continue;

				ResourcesToLoad.AddScene(charaPath);
				loadedCharacters.Add(curCharacter);
			}
			
			return;
		}
		
		string eventsPath = ResourcesToLoad.GetResourcePath($"res://songs/{context.Name}/data/Events");
		if (path == eventsPath)
		{
			Resource eventsResource = ResourceLoader.LoadThreadedGet(eventsPath);
			if (eventsResource is not EventMeta eventMeta)
				return;

			RubiconGame.Events = eventMeta;
			
			List<string> eventsPassed = [];
			for (int i = 0; i < eventMeta.Events.Length; i++)
			{
				string eventName = eventMeta.Events[i].Name;
				if (eventsPassed.Contains(eventName))
					continue;
				
				eventsPassed.Add(eventName);
				ResourcesToLoad.AddScene($"res://resources/game/events/{eventName}");
			}

			return;
		}
		
		string ruleSetPath = PathUtility.GetResourcePath($"res://resources/game/rulesets/{context.RuleSet}");
		if (path == ruleSetPath)
		{
			string noteSkinPath = $"res://resources/ui/styles/{RubiconGame.Metadata.NoteSkin}/{context.RuleSet}";
			if (!PathUtility.ResourceExists(noteSkinPath))
				noteSkinPath = $"res://resources/ui/styles/{ProjectSettings.GetSetting($"rubicon/rulesets/{context.RuleSet.ToLower()}/default_note_skin")}/{RubiconGame.Context.RuleSet}";

			ResourcesToLoad.AddResource(noteSkinPath);
		}
	}

	public override void _Ready()
	{
		Name = "RubiconGameScreen";
		
		#if TOOLS
		if (RubiconGame.Context == null)
			RubiconGame.Context = new LoadContext() { Name = SongName, Difficulty = Difficulty, RuleSet = RuleSet };
		#endif

		base._Ready();
		
		if (!IsLoaded())
			return;
		
		RubiconGame.Setup(this);
	}
}