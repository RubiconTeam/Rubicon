/*using Godot.Collections;
using Rubicon.Data.Settings.Attributes;
using Array = Godot.Collections.Array;

namespace Rubicon.Data.Settings;

public class SettingsStorage
{
	public Gameplay Gameplay { get; set; } = new();
	public Audio Audio { get; set; } = new();
	public Video Video { get; set; } = new();
	public Misc Misc { get; set; } = new();
	public Keybinds Keybinds { get; set; } = new();
}

public class Gameplay
{
	public bool DownScroll { get; set; }
	public bool CenterBarLine { get; set; } = false;
	public bool GhostTapping { get; set; } = true;
	public bool FlashingLights { get; set; } = true;
	
	[RubiconSettingsGroup("Gameplay Modifiers")]
	public class GameplayModifiers
	{
		[StepValue(0.01f, 1f, 1f)] public double PlaybackRate { get; set; } = 1f;
		[StepValue(0.1f, 1f, 1f)] public double HealthGain { get; set; } = 1f;
		[StepValue(0.1f, 1f, 1f)] public double HealthLoss { get; set; } = 1f;
		[StepValue(0.1f, 1f, 1f)] public double HealthDrain { get; set; } = 0.5f;
		public bool OpponentDrainsHealth { get; set; } = false;
	}
}

public class Audio
{
	public double AudioOffset { get; set; } = 0f;
	public double MasterVolume { get; set; } = 100f;
	public double InstVolume { get; set; } = 100f;
	public double VoicesVolume { get; set; } = 100f;
	public double SfxVolume { get; set; } = 100f;
	public double MusicVolume { get; set; } = 100f;
}

public class Video
{
	[ProjectSetting("display/window/size/mode")] public DisplayServer.WindowMode WindowMode { get; set; }
	[ProjectSetting("display/window/vsync/vsync_mode")] public DisplayServer.VSyncMode VSync { get; set; }
	[ProjectSetting("application/run/max_fps"), StepValue(1f, 10, 0)] public int MaxFPS { get; set; }
}

public class Keybinds
{
	public Dictionary UiKeybinds { get; set; } = new()
	{
		["UI_DOWN"] = "Down",
		["UI_UP"] = "Up",
		["UI_LEFT"] = "Left",
		["UI_RIGHT"] = "Right",
		["UI_CANCEL"] = "Escape",
		["UI_ENTER"] = "Enter",
	};
	public Dictionary GameplayKeybinds { get; set; } = new();	
}*/
