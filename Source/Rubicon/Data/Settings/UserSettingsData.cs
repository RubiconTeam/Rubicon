using Godot.Collections;
using Rubicon.Data.Settings.Attributes;

namespace Rubicon.Data.Settings;

public partial class UserSettingsData
{
    public GameplaySection Gameplay = new();
    public VideoSection Video = new();
    public AudioSection Audio = new();
    public MiscSection Misc = new();
    public InputMapSection Bindings = new();

    /// <summary>
    /// Loads all valid settings from a <see cref="ConfigFile"/>.
    /// </summary>
    /// <param name="config">The config file to input</param>
    public partial void Load(ConfigFile config);

    /// <summary>
    /// Creates a new instance of <see cref="ConfigFile"/>, populated with the current settings.
    /// </summary>
    /// <returns></returns>
    public partial ConfigFile CreateConfigFileInstance();
    
    /// <summary>
    /// Gets a setting by key. More useful in GDScript than it is in C#.
    /// </summary>
    /// <param name="key">The key (case-sensitive)</param>
    /// <returns>The variant value if found, null if not.</returns>
    public partial Variant GetSetting(string key);

    /// <summary>
    /// Sets a setting by key. More useful in GDScript than it is in C#.
    /// </summary>
    /// <param name="key">The key (case-sensitive)</param>
    /// <param name="val">The variant value to set this setting to</param>
    public partial void SetSetting(string key, Variant val);
}


[RubiconSettingsSection("Gameplay", true, "res://Assets/UI/Menus/Settings/Gameplay.png")]
public class GameplaySection
{
    public double Offset = 0.0d;
    public double VisualOffset = 0.0d;
    public bool DownScroll = false;
    public bool CenterBarLine = false;
    public bool GhostTapping = false;
    public bool FlashingLights = true;
    public bool Autoplay = false;
    public float NoteAmplitude = 0f; // 0 means set to the default chart note amplitude (scroll speed)

    [RubiconSettingsGroup("Gameplay Modifiers")]
    public class GameplayModifiers
    {
        [StepValue(0.01f, 1f, 1f)] public double PlaybackRate = 1d;
        [StepValue(0.1f, 1f, 1f)] public double HealthGain = 1d;
        [StepValue(0.1f, 1f, 1f)] public double HealthLoss = 1d;
        [StepValue(0.1f, 1f, 1f)] public double HealthDrain = 0.5d;
        public bool OpponentDrainsHealth { get; set; } = false;
    }
}

[RubiconSettingsSection("Video", true, "res://Assets/UI/Menus/Settings/Video.png")]
public class VideoSection
{
    [ProjectSetting("display/window/size/mode")] public Window.ModeEnum Fullscreen = (Window.ModeEnum)ProjectSettings.GetSetting("display/window/size/mode").AsInt64();
    [ProjectSetting("display/window/size/window_width_override")] public Vector2I Resolution = new(ProjectSettings.GetSetting("display/window/size/window_width_override").AsInt32(), ProjectSettings.GetSetting("display/window/size/window_height_override").AsInt32());
    [ProjectSetting("display/window/vsync/vsync_mode")] public DisplayServer.VSyncMode VSync = (DisplayServer.VSyncMode)ProjectSettings.GetSetting("display/window/vsync/vsync_mode").AsInt64();
    [ProjectSetting("application/run/max_fps")] public int MaxFps = ProjectSettings.GetSetting("application/run/max_fps").AsInt32();

    public Settings3DSection Settings3D = new();
    
    [RubiconSettingsGroup("3D Settings")]
    public class Settings3DSection
    {
        [ProjectSetting("rendering/scaling_3d/scale")] public Viewport.Scaling3DModeEnum Scaling3DMode = (Viewport.Scaling3DModeEnum)ProjectSettings.GetSetting("rendering/scaling_3d/scale").AsInt64();
        [ProjectSetting("rendering/scaling_3d/scale")] public float RenderScale = ProjectSettings.GetSetting("rendering/scaling_3d/scale").AsSingle();
        [ProjectSetting("rendering/scaling_3d/fsr_sharpness")] public float FsrSharpness = ProjectSettings.GetSetting("rendering/scaling_3d/fsr_sharpness").AsSingle();
    }
}

[RubiconSettingsSection("Audio", true, "res://Assets/UI/Menus/Settings/Audio.png")]
public class AudioSection
{
    public float MasterVolume = 1.0f;
    public float MusicVolume = 1.0f;
    public float VocalsVolume = 1.0f;
    public float SfxVolume = 1.0f;
}

[RubiconSettingsSection("Misc", true, "res://Assets/UI/Menus/Settings/Misc.png")]
public class MiscSection
{
    public DebugMiscSettings Debug = new();
	
    [RubiconSettingsGroup("Debug Settings")]
    public class DebugMiscSettings
    {
        public bool PrintSettingsOnConsole;
    }
}

[RubiconSettingsSection("Keybinds", true, "res://Assets/UI/Menus/Settings/Keybinds.png")]
public class InputMapSection
{
    public Dictionary<string, Array<InputEvent>> Map = RubiconEngine.DefaultInputMap.Duplicate();
}