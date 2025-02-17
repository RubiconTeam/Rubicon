using Godot.Collections;
using Rubicon;

namespace GodotSharp.Utilities;

public partial class UserSettingsData
{
    public GameplaySection Gameplay;
    public VideoSection Video;
    public AudioSection Audio;
    public MiscSection Misc;
    public InputMapSection Bindings;

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

    /// <summary>
    /// Gets all the sections present.
    /// </summary>
    /// <returns>All sections</returns>
    public partial string[] GetSections();
    
    /// <summary>
    /// Gets all present keys in the section provided.
    /// </summary>
    /// <param name="section">The section</param>
    /// <returns>An array of keys if section is found, empty array if not.</returns>
    public partial string[] GetSectionKeys(string section);
    
    /// <summary>
    /// Gets all attributes for a setting.
    /// </summary>
    /// <param name="key">The setting</param>
    /// <returns>An array of one or multiple attribute data.</returns>
    public partial UserSettingAttributeData[] GetAttributesForSetting(string key);
}

[RubiconSettingsSection(name: "Gameplay", generateInMenu: true, iconPath: "res://assets/ui/menus/settings/gameplay.png", sectionName: "Gameplay")]
public class GameplaySection
{
    [StepValue(0.01f, 1f, 1f)] 
    public double Offset = 0d;
    
    [StepValue(0.01f, 1f, 1f)] 
    public double VisualOffset = 0d;
    
    [StepValue(0.01f, 1f, 1f)] 
    public double SpeedMultiplier = 1d;
    
    public bool DownScroll = false;
    public bool CenterBarLine = false;
    public bool GhostTapping = false;
    public bool FlashingLights = true;
    public bool Autoplay = false;
}

[RubiconSettingsSection(name: "Video", generateInMenu: true, iconPath: "res://assets/ui/menus/settings/video.png", sectionName: "Video")]
public class VideoSection
{
    [ProjectSetting("display/window/size/mode")] 
    public Window.ModeEnum Fullscreen;
    
    [ProjectSetting("rubicon/general/starting_window_size")] 
    public Vector2I Resolution;
    
    [ProjectSetting("display/window/vsync/vsync_mode")] 
    public DisplayServer.VSyncMode VSync;
    
    [ProjectSetting("application/run/max_fps")] 
    public int MaxFps;

    public Settings3DSection Settings3D;
    
    [RubiconSettingsGroup("3D Settings")]
    public class Settings3DSection
    {
        [ProjectSetting("rendering/scaling_3d/scale")] 
        public Viewport.Scaling3DModeEnum Scaling3DMode;
        
        [ProjectSetting("rendering/scaling_3d/fsr_sharpness")] 
        public float FsrSharpness;
    }
}

[RubiconSettingsSection(name: "Audio", generateInMenu: true, iconPath: "res://assets/ui/menus/settings/audio.png", sectionName: "Audio")]
public class AudioSection
{
    [StepValue(1, 0f, 100f)] 
    public double MasterVolume = 1.0;
    
    [StepValue(1, 0f, 100f)] 
    public double MusicVolume = 1.0;
    
    [StepValue(1, 0f, 100f)] 
    public double SfxVolume = 1.0;
}

[RubiconSettingsSection(name: "Miscellaneous", generateInMenu: true, iconPath: "res://assets/ui/menus/settings/Miscellaneous.png", sectionName: "Miscellaneous")]
public class MiscSection
{
    public bool PrintErrorsOnScreen = false;
}

[RubiconSettingsSection(name: "Keybinds", generateInMenu: true, iconPath: "res://assets/ui/menus/settings/Keybinds.png", sectionName: "Keybinds")]
public class InputMapSection
{
    public Dictionary<string, Array<InputEvent>> Map;
}