using System.Linq;
using System.Text;
using Rubicon.Core.Chart;

namespace Rubicon.Core.Autoload;

public partial class DebugInfo : CanvasLayer
{
	/*Main Info (Always Visible)*/
	//Performance
	[Export] private Label FPS;
	[Export] private Label RAM;
	[Export] private Label VRAM;
	
	/*Debug Info (Keybind Based)*/
	//Scene Tree
	[Export] private Label AllObjects;
	[Export] private Label NodeObjects;
	[Export] private Label ResourceObjects;
	
	//Versions
	[Export] private Label GameVersion;
	[Export] private Label RubiconVersion;
	[Export] private Label GodotVersion;
	
	//Misc
	[Export] private Label CurrentScene;
	[Export] private Label ConductorInfo;

	/*Visibility Shit*/
	[Export] private VBoxContainer DebugInformation; 
	
	private float RAMUpdateTime;
	private float ObjectUpdateTime;
	
	public override void _Ready()
	{
		this.OnReady();
		if (!OS.IsDebugBuild()) VRAM.Visible = false;
		DebugInformation.VisibilityChanged += () =>
		{
			if (!DebugInformation.Visible) return;
			UpdateObjects();
			UpdateScene();
		};
		DebugInformation.Visible = false;
		UpdateStaticLabels();
	}
	
	private static string ConvertToMemoryFormat(ulong mem)
	{
		if (mem >= 0x40000000)
			return (float)Math.Round(mem / 1024f / 1024f / 1024f, 2) + " GB";
		if (mem >= 0x100000)
			return (float)Math.Round(mem / 1024f / 1024f, 2) + " MB";
		if (mem >= 0x400)
			return (float)Math.Round(mem / 1024f, 2) + " KB";

		return mem + " B";
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("debug_toggle")) 
			DebugInformation.Visible = !DebugInformation.Visible;
		
		UpdateFPS();
		
		RAMUpdateTime += (float)delta;
		if (RAMUpdateTime >= 1f)
		{
			UpdateRAM();
			if (VRAM.Visible) UpdateVRAM();
			RAMUpdateTime = 0f;
		}

		if (!DebugInformation.Visible) return;
		
		ObjectUpdateTime += (float)delta;
		if (ObjectUpdateTime >= 2f)
		{
			UpdateObjects();
			UpdateScene();
			CurrentScene.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";
			ObjectUpdateTime = 0f;
		}

		if (Conductor.Singleton != null)
		{
			UpdateConductor();
			ConductorInfo.Visible = true;
		}
		else ConductorInfo.Visible = false;
	}

	private string GetKeybinds(Node node) => string.Join(", ", InputMap.ActionGetEvents(node.Name).OfType<InputEventKey>().Select(key => key.AsTextPhysicalKeycode()));

	private void UpdateStaticLabels()
	{
		GameVersion.Text = $"{ProjectSettings.GetSetting("application/config/name").AsString()} {ProjectSettings.GetSetting("application/config/version").AsString()} {(OS.IsDebugBuild() ? "[Debug]" : "[Release]")}";
		RubiconVersion.Text = $"Rubicon Engine {RubiconEngineInstance.Version}";
		GodotVersion.Text = $"Godot Engine {Engine.GetVersionInfo()["major"]}.{Engine.GetVersionInfo()["minor"]}.{Engine.GetVersionInfo()["patch"]} [{Engine.GetVersionInfo()["status"]}]";
	}

	private void UpdateFPS() => FPS.Text = $"FPS: {Mathf.FloorToInt(1 / GetProcessDeltaTime())}";

	private void UpdateRAM() => RAM.Text = $"RAM: {ConvertToMemoryFormat(OS.GetStaticMemoryUsage())} [{ConvertToMemoryFormat(OS.GetStaticMemoryPeakUsage())}]";

	private void UpdateVRAM() => VRAM.Text = $"VRAM: {ConvertToMemoryFormat((ulong)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed))}";

	private void UpdateScene() => CurrentScene.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";

	private void UpdateObjects()
	{
		AllObjects.Text = $"All Instantiated Objects: {Performance.GetMonitor(Performance.Monitor.ObjectCount)}";
		ResourceObjects.Text = $"Resource Objects in Use: {Performance.GetMonitor(Performance.Monitor.ObjectResourceCount)}";
		NodeObjects.Text = $"Node Objects: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)} (Orphan Nodes: {Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount)})";
	}

	private readonly StringBuilder ConductorSB = new();

	private void UpdateConductor()
	{
		ConductorSB.Clear();

		ConductorSB.AppendLine($"BPM: {Conductor.Bpm}, Audio Position: {Conductor.RawTime}\n");
		foreach (BpmInfo bpm in Conductor.BpmList)
			ConductorSB.AppendLine($"Time: {bpm.Time}, Exact Time (ms): {bpm.MsTime}, BPM: {bpm.Bpm}, Time Signature: {bpm.TimeSignatureNumerator}/{bpm.TimeSignatureDenominator}\n")
			.AppendLine($"Step: {Conductor.CurrentStep}")
			.AppendLine($"Beat: {Conductor.CurrentBeat}")
			.AppendLine($"Measure: {Conductor.CurrentMeasure}");

		ConductorInfo.Text = ConductorSB.ToString();
	}
}
