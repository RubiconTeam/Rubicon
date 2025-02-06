using System.Collections.Generic;
using Rubicon.Core;
using Rubicon.Core.Data;
using Rubicon.Core.Events;
using Rubicon.Core.Meta;
using Rubicon.Data;
using Rubicon.Screens;

namespace Rubicon.Game;

[GlobalClass] public partial class RubiconGameScreen : CsScreen
{
#if TOOLS
	[Export] public string SongName = ProjectSettings.GetSetting("rubicon/general/fallback/song").AsString();
	
	[Export] public string Difficulty = ProjectSettings.GetSetting("rubicon/general/fallback/difficulty").AsString();
	
	[Export] public string RuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
#endif
	
    private bool _preloaded = false;
    
    public override void ReadyPreload()
	{
		_preloaded = true;
		
		// Just load the song meta for now, we'll wait until it's loaded.
		LoadContext context = RubiconGame.Context;
		ResourcesToLoad.AddResource($"res://Songs/{context.Name}/Data/Meta");
		ResourcesToLoad.AddPath($"res://Songs/{context.Name}/Data/{context.RuleSet}-{context.Difficulty}.rbc");

		string eventsPath = $"res://Songs/{context.Name}/Data/Events";
		if (PathUtility.ResourceExists(eventsPath))
			ResourcesToLoad.AddResource($"res://Songs/{context.Name}/Data/Events");

		List<string> scriptPaths = new List<string>();
		scriptPaths.AddRange(PathUtility.GetAbsoluteFilePathsAt("res://Resources/Game/Common/", true));
		scriptPaths.AddRange(PathUtility.GetAbsoluteFilePathsAt($"res://Songs/{context.Name}/Scripts/", true));
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
		string metaPath = ResourcesToLoad.GetResourcePath($"res://Songs/{context.Name}/Data/Meta");
		if (path == metaPath) // Meta loaded
		{
			Resource metaResource = ResourceLoader.LoadThreadedGet(metaPath);
			if (metaResource is not SongMeta meta)
				return;

			RubiconGame.Metadata = meta;
			string uiStylePath = $"res://Resources/UI/Styles/{meta.UiStyle}/Style";
			if (!PathUtility.ResourceExists(uiStylePath))
				uiStylePath = $"res://Resources/UI/Styles/{ProjectSettings.GetSetting("rubicon/general/default_ui_style")}/Style";
				
			ResourcesToLoad.AddResource(uiStylePath);
			
			string ruleSetLoadPath = $"res://Resources/Game/Rulesets/{context.RuleSet}";
			if (!PathUtility.ResourceExists(ruleSetLoadPath))
			{
				context.RuleSet = ProjectSettings.GetSetting("rubicon/rulesets/default_ruleset").AsString();
				ruleSetLoadPath = $"res://Resources/Game/Rulesets/{context.RuleSet}";
			}
			
			ResourcesToLoad.AddResource(ruleSetLoadPath);
			
			if (meta.Environment == GameEnvironment.None)
				return;
			
			string envSuffix = meta.Environment == GameEnvironment.CanvasItem ? "2d" : "3d";
			string stagePath = $"res://Resources/Game/Stages/{meta.Stage}";
			if (!PathUtility.SceneExists(stagePath)) // Stage Fallback
			{
				string fallBackStage = ProjectSettings.GetSetting("rubicon/general/fallback/stage_" + envSuffix).AsString();
				stagePath = $"res://Resources/Game/Stages/{fallBackStage}";
				if (!PathUtility.SceneExists(stagePath)) // Dude
					return;
			}
			
			ResourcesToLoad.AddScene(stagePath);
			
			List<string> loadedCharacters = new List<string>();
			for (int i = 0; i < meta.Characters.Length; i++)
			{
				string curCharacter = meta.Characters[i].Character;
				if (loadedCharacters.Contains(curCharacter))
					continue;

				string charaPath = $"res://Resources/Game/Characters/{curCharacter}";
				if (!PathUtility.SceneExists(charaPath)) // Fallback
				{
					curCharacter = ProjectSettings.GetSetting("rubicon/general/fallback/character_" + envSuffix).AsString();	
					charaPath = $"res://Resources/Game/Characters/{curCharacter}";
					
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
		
		string eventsPath = ResourcesToLoad.GetResourcePath($"res://Songs/{context.Name}/Data/Events");
		if (path == eventsPath)
		{
			Resource eventsResource = ResourceLoader.LoadThreadedGet(eventsPath);
			if (eventsResource is not EventMeta eventMeta)
				return;

			RubiconGame.Events = eventMeta;
			
			List<string> eventsPassed = new List<string>();
			for (int i = 0; i < eventMeta.Events.Length; i++)
			{
				string eventName = eventMeta.Events[i].Name;
				if (eventsPassed.Contains(eventName))
					continue;
				
				eventsPassed.Add(eventName);
				ResourcesToLoad.AddScene($"res://Resources/Game/Events/{eventName}");
			}

			return;
		}
		
		string ruleSetPath = PathUtility.GetResourcePath($"res://Resources/Game/Rulesets/{context.RuleSet}");
		if (path == ruleSetPath)
		{
			string noteSkinPath = $"res://Resources/UI/Styles/{RubiconGame.Metadata.NoteSkin}/{context.RuleSet}";
			if (!PathUtility.ResourceExists(noteSkinPath))
				noteSkinPath = $"res://Resources/UI/Styles/{ProjectSettings.GetSetting($"rubicon/rulesets/{context.RuleSet.ToLower()}/default_note_skin")}/{RubiconGame.Context.RuleSet}";

			ResourcesToLoad.AddResource(noteSkinPath);
		}
	}

	public override void _Ready()
	{
		base._Ready();
		Name = "RubiconGameScreen";
		
		#if TOOLS
		if (RubiconGame.Context == null)
			RubiconGame.Context = new LoadContext() { Name = SongName, Difficulty = Difficulty, RuleSet = RuleSet };
		#endif
		
		if (!_preloaded)
		{
			ScreenManager.SwitchScreen(GetSceneFilePath(), "Default");
			return;
		}
		
		RubiconGame.Setup(this);
	}
}