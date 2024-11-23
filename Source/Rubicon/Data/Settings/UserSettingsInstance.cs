using Godot.Collections;
using Rubicon.Data.Generation;
using Rubicon.Data.Settings;

namespace Rubicon.Data.Settings;

[GlobalClass, StaticAutoloadSingleton("Rubicon.Data.Settings", "UserSettings")]
public partial class UserSettingsInstance : Node
{
	private UserSettingsData _data;

	public override void _Ready()
	{
		if (Load() != Error.Ok)
		{
			Reset();
			Save();
		}
		
		UpdateSettings();
		UpdateKeybinds();
	}

	public void UpdateSettings()
	{
		// Video
		Window mainWindow = GetTree().GetRoot();
		mainWindow.Mode = Video.Fullscreen;
		mainWindow.Size = Video.Resolution;
		DisplayServer.WindowSetVsyncMode(Video.VSync);
		Engine.MaxFps = Video.MaxFps;
		mainWindow.Scaling3DMode = Video.Settings3D.Scaling3DMode;
		mainWindow.Scaling3DScale = Video.Settings3D.RenderScale;
		mainWindow.FsrSharpness = Video.Settings3D.FsrSharpness;
	}

	public void UpdateKeybinds()
	{
		foreach (var bind in Bindings.Map)
		{
			string curAction = bind.Key;
			Array<InputEvent> events = bind.Value;
			
			InputMap.ActionEraseEvents(curAction);
			
			for (int i = 0; i < events.Count; i++)
				InputMap.ActionAddEvent(curAction, events[i]);
		}
	}
	
	public Error Load(string path = null)
	{
		path ??= ProjectSettings.GetSetting("rubicon/general/settings_save_path").AsString();
		if (!FileAccess.FileExists(path))
			return Error.FileNotFound;
		
		ConfigFile config = new();
		Error loadError = config.Load(path);
		if (loadError != Error.Ok)
			return loadError;

		Reset();
		_data.Load(config);
		return Error.Ok;
	}

	public Error Save(string path = null)
	{
		path ??= ProjectSettings.GetSetting("rubicon/general/settings_save_path").AsString();
		ConfigFile configFile = _data.CreateConfigFileInstance();
		
		return configFile.Save(path);
	}

	public void Reset()
	{
		_data = new UserSettingsData();
	}

	public Variant GetSetting(string key) => _data.GetSetting(key);

	public void SetSetting(string key, Variant value) => _data.SetSetting(key, value);
}
