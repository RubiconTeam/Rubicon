/*using Godot.Collections;
using Rubicon.Data.Settings;
using Rubicon.Data.Settings.Attributes;
using Array = Godot.Collections.Array;

[GlobalClass]
public partial class SettingsManager : Node
{
	[Export] public ConfigFile Data = new();
	[Export] public string SettingsFilePath = "user://settings.cfg";
	
	public static SettingsStorage Instance { get; private set; } = new();
	public static Gameplay Gameplay { get; private set; } = Instance.Gameplay;
	public static Audio Audio { get; private set; } = Instance.Audio;
	public static Video Video { get; private set; } = Instance.Video;
	public static Misc Misc { get; private set; } = Instance.Misc;
	public static Keybinds Keybinds { get; private set; } = Instance.Keybinds;
	
	[Signal] public delegate void SettingsLoadedEventHandler(string settings);
	
	public override void _Ready()
	{
		base._Ready();
		
		if (!Load())
		{
			Reset();
			Save();
		}
		CallDeferred(MethodName.EmitSettingsLoaded);
	}
	
	public bool Load()
	{
		if (FileAccess.FileExists(SettingsFilePath))
		{
			string configText = FileAccess.GetFileAsString(SettingsFilePath);
			if (Data.Parse(configText) == Error.Ok)
			{
				LoadSectionFromFile(Instance.Gameplay);
				LoadSectionFromFile(Instance.Audio);
				LoadSectionFromFile(Instance.Video);
				LoadSectionFromFile(Instance.Misc);
				LoadSectionFromFile(Instance.Keybinds);
				return true;
			}
		}
		return false;
	}
	
	public bool Save()
	{
		SerializeSettings();
		if (Data.Save(SettingsFilePath) == Error.Ok)
			return true;

		return false;
	}

	public void Reset()
	{
		Instance = new SettingsStorage();
		SerializeSettings();
	}

	private void SerializeSettings()
	{
		SerializeSection(Instance.Gameplay);
		SerializeSection(Instance.Audio);
		SerializeSection(Instance.Video);
		SerializeSection(Instance.Misc);
		SerializeSection(Instance.Keybinds);
	}

	private void SerializeSection(object section, string parentSection = "")
	{
		var type = section.GetType();
		var attribute = (RubiconSettingsSectionAttribute)Attribute.GetCustomAttribute(type, typeof(RubiconSettingsSectionAttribute));
		string sectionName = string.IsNullOrEmpty(parentSection) ? attribute?.Name : parentSection;

		foreach (var property in type.GetProperties())
		{
			object value = property.GetValue(section);
			if (value == null) continue;
			if (!(value.GetType().IsClass && value.GetType() != typeof(string) && !typeof(Dictionary).IsAssignableFrom(value.GetType()) && !typeof(Array).IsAssignableFrom(value.GetType())))
			{
				Variant variantValue = value switch
				{
					bool b => b,
					int i => i,
					float f => f,
					string s => s,
					double d => d,
					Dictionary d => d,
					Array a => a,
					Enum e => Convert.ToInt32(e),
					_ => throw new InvalidOperationException($"Unsupported type: {value.GetType()}")
				};

				Data.SetValue(sectionName, property.Name, variantValue);
			}
		}

		foreach (var property in type.GetProperties())
		{
			object value = property.GetValue(section);
			if (value == null) continue;
			if (value.GetType().IsClass && value.GetType() != typeof(string) && !typeof(Dictionary).IsAssignableFrom(value.GetType()) && !typeof(Array).IsAssignableFrom(value.GetType())) 
				SerializeSection(value, sectionName + "/" + property.Name);
		}
	}

	private void LoadSectionFromFile(object section, string parentSection = "")
	{
		var type = section.GetType();
		var attribute = (RubiconSettingsSectionAttribute)Attribute.GetCustomAttribute(type, typeof(RubiconSettingsSectionAttribute));
		string sectionName = attribute != null ? attribute.Name : parentSection;

		foreach (var property in type.GetProperties())
		{
			if (property.PropertyType.IsClass && property.PropertyType != typeof(string) && !typeof(Dictionary).IsAssignableFrom(property.PropertyType) && !typeof(Array).IsAssignableFrom(property.PropertyType))
			{
				var nestedObject = property.GetValue(section);
				if (nestedObject == null)
				{
					//shut up
					#pragma warning disable IL2072
					nestedObject = Activator.CreateInstance(property.PropertyType);
					#pragma warning restore IL2072
					property.SetValue(section, nestedObject);
				}

				LoadSectionFromFile(nestedObject, sectionName + "/" + property.Name);
			}
			else if (Data.HasSectionKey(sectionName, property.Name))
			{
				Variant variantValue = Data.GetValue(sectionName, property.Name);
				if (property.PropertyType.IsEnum) property.SetValue(section, Enum.ToObject(property.PropertyType, variantValue));
				else
				{
					switch (variantValue.VariantType)
					{
						case Variant.Type.Int:
							property.SetValue(section, variantValue.AsInt32());
							break;
						case Variant.Type.Float:
							property.SetValue(section, variantValue.AsDouble());
							break;
						case Variant.Type.Bool:
							property.SetValue(section, variantValue.AsBool());
							break;
						case Variant.Type.String:
							property.SetValue(section, variantValue.AsString());
							break;
						case Variant.Type.Array:
							property.SetValue(section, variantValue.AsGodotArray());
							break;
						case Variant.Type.Dictionary:
							property.SetValue(section, variantValue.AsGodotDictionary());
							break;
					}
				}
			}
		}
	}
	
	private void EmitSettingsLoaded() => EmitSignal(SignalName.SettingsLoaded, SettingsFilePath);
}*/
