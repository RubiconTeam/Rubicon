using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot.Collections;
using Rubicon.Data.Generation;
using Rubicon.Data.Settings.Attributes;
using Array = Godot.Collections.Array;

namespace Rubicon.Data.Settings;

[GlobalClass, StaticAutoloadSingleton("Rubicon.Data.Settings", "UserSettings")]
public partial class UserSettingsInstance : Node
{
	public static UserSettingsData _data { get; set; }

	public override void _Ready()
	{
		if (Load() != Error.Ok)
		{
			GD.Print("Failed to load settings. Resetting to defaults.");
			Reset();
			Save();
		}

		UpdateSettings();
		UpdateKeybinds();
	}

	public void UpdateSettings()
	{
		GD.Print("Updating settings...");
		try
		{
			Window mainWindow = GetTree().GetRoot();
			mainWindow.Mode = Video.Fullscreen;
			DisplayServer.WindowSetVsyncMode(Video.VSync);
			Engine.MaxFps = Video.MaxFps;
			mainWindow.Scaling3DMode = Video.Settings3D.Scaling3DMode;
			mainWindow.FsrSharpness = Video.Settings3D.FsrSharpness;

			GD.Print("Settings updated successfully.");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error updating settings: {ex.Message}");
		}
	}

	public void UpdateKeybinds()
	{
		GD.Print("Updating keybinds...");
		try
		{
			foreach (var bind in Bindings.Map)
			{
				string curAction = bind.Key;
				Array<InputEvent> events = bind.Value;

				InputMap.ActionEraseEvents(curAction);

				for (int i = 0; i < events.Count; i++)
				{
					InputMap.ActionAddEvent(curAction, events[i]);
				}

				GD.Print($"Keybind updated: {curAction} with {events.Count} events.");
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error updating keybinds: {ex.Message}");
		}
	}

	public Error Load(string path = null)
	{
		GD.Print("Loading settings...");
		path ??= ProjectSettings.GetSetting("rubicon/general/settings_save_path").AsString();
		GD.Print($"Settings file path: {path}");

		if (!FileAccess.FileExists(path))
		{
			GD.PrintErr("Settings file not found.");
			return Error.FileNotFound;
		}

		ConfigFile config = new();
		Error loadError = config.Load(path);
		if (loadError != Error.Ok)
		{
			GD.PrintErr($"Failed to load settings file: {loadError}");
			return loadError;
		}

		Reset();

		GD.Print("Iterating over UserSettingsData fields...");
		foreach (var field in typeof(UserSettingsData).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) 
			ProcessField(field, config);
		
		return Error.Ok;
	}

	private void ProcessField(FieldInfo field, ConfigFile config)
	{
		try
		{
			GD.Print($"Processing field: {field.Name}, Type: {field.FieldType.Name}");
			var (targetInstance, _) = GetOrCreateInstanceChain(field.DeclaringType, _data);

			if (field.FieldType.IsClass)
			{
				field.SetValue(targetInstance, field.GetValue(targetInstance));
				foreach (var nestedField in field.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) 
					ProcessField(nestedField, config);
			}
			else
			{
				var projectSettingAttribute = field.GetCustomAttribute<ProjectSettingAttribute>();
				if (projectSettingAttribute != null)
				{
					string settingKey = projectSettingAttribute.SettingName;
					GD.Print($"Checking ProjectSetting: {settingKey}");

					if (ProjectSettings.HasSetting(settingKey))
					{
						Variant value = ProjectSettings.GetSetting(settingKey);
						GD.Print($"Loaded ProjectSetting: {settingKey} = {value}");
						field.SetValue(targetInstance, ConvertVariant(value, field.FieldType));
					}
				}
				else
				{
					var sectionAttribute = field.DeclaringType?.GetCustomAttribute<RubiconSettingsSectionAttribute>();
					var groupAttribute = field.GetCustomAttribute<RubiconSettingsGroupAttribute>();

					if (sectionAttribute != null)
					{
						string sectionName = sectionAttribute.Name;
						if (groupAttribute != null)
						{
							string groupPath = groupAttribute.SectionName.Replace(" ", "/");
							sectionName += $"/{groupPath}";
						}

						string configKey = $"{sectionName}/{field.Name}";
						GD.Print($"Checking ConfigFile key: [{sectionAttribute.Name}] {configKey}");

						if (config.HasSectionKey(sectionAttribute.Name, configKey))
						{
							Variant value = config.GetValue(sectionAttribute.Name, configKey);
							GD.Print($"Loaded ConfigFile key: [{sectionAttribute.Name}] {configKey} = {value}");
							field.SetValue(targetInstance, ConvertVariant(value, field.FieldType));
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error processing field '{field.Name}': {ex.Message}");
		}
	}
	
	private (object instance, List<object> parentChain) GetOrCreateInstanceChain(Type targetType, object rootInstance)
	{
		var parentChain = new List<object> { rootInstance };
		object currentInstance = rootInstance;

		if (targetType == typeof(UserSettingsData))
			return (rootInstance, parentChain);
		
		var typePath = new List<Type>();
		var currentType = targetType;

		while (currentType != null && currentType != typeof(UserSettingsData))
		{
			typePath.Insert(0, currentType);
			currentType = currentType.DeclaringType;
		}

		foreach (var type in typePath)
		{
			if (currentInstance != null)
			{
				var parentType = currentInstance.GetType();
				var field = parentType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
					.FirstOrDefault(f => f.FieldType == type || f.FieldType.IsSubclassOf(type));

				if (field == null)
				{
					throw new InvalidOperationException($"Could not find field of type {type.Name} in {parentType.Name}");
				}

				var nextInstance = field.GetValue(currentInstance);
				if (nextInstance == null)
				{
					nextInstance = Activator.CreateInstance(type);
					field.SetValue(currentInstance, nextInstance);
				}

				currentInstance = nextInstance;
			}

			parentChain.Add(currentInstance);
		}

		return (currentInstance, parentChain);
	}
	
	public Error Save(string path = null)
	{
		GD.Print("Saving settings...");
		path ??= ProjectSettings.GetSetting("rubicon/general/settings_save_path").AsString();
		GD.Print($"Saving settings to: {path}");

		ConfigFile configFile = _data.CreateConfigFileInstance();
		return configFile.Save(path);
	}

	private object ConvertVariant(Variant value, Type targetType)
	{
		try
		{
			if (targetType.IsEnum)
			{
				return Enum.ToObject(targetType, value.AsInt32());
			}

			return targetType.Name switch
			{
				nameof(Int32) => value.AsInt32(),
				nameof(Single) => value.AsSingle(),
				nameof(Boolean) => value.AsBool(),
				nameof(String) => value.AsString(),
				nameof(Vector2I) => value.AsVector2I(),
				nameof(Array) => value.AsGodotArray(),
				nameof(Dictionary) => value.AsGodotDictionary(),
				_ => throw new InvalidCastException($"Unsupported type: {targetType.Name}")
			};
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Error converting Variant: {value} to {targetType.Name} - {ex.Message}");
			throw;
		}
	}

	public void Reset()
	{
		_data = new UserSettingsData();
	}

	public Variant GetSetting(string key)
	{
		return _data.GetSetting(key);
	}

	public void SetSetting(string key, Variant value)
	{
		_data.SetSetting(key, value);
	}
}
