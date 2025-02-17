using System.Linq;
using System.Text;
using Rubicon.Core;
using Rubicon.Core.Chart;

namespace Rubicon.Debug;

public partial class DebugTools : CanvasLayer
{
	/*Main Info (Always Visible)*/
	//Performance
	[Export] private Label _fps;
	[Export] private Label _ram;
	[Export] private Label _vram;
	
	/*Debug Info (Keybind Based)*/
	//Scene Tree
	[Export] private Label _allObjects;
	[Export] private Label _nodeObjects;
	[Export] private Label _resourceObjects;
	
	//Versions
	[Export] private Label _gameVersion;
	[Export] private Label _rubiconVersion;
	[Export] private Label _godotVersion;
	
	//Misc
	[Export] private Label _currentScene;
	[Export] private Label _conductorInfo;

	/*Visibility Shit*/
	[Export] private VBoxContainer _debugInformation; 
	
	private float _ramUpdateTime;
	private float _objectUpdateTime;
	
	public override void _Ready()
	{
		if (!OS.IsDebugBuild()) _vram.Visible = false;
		_debugInformation.VisibilityChanged += () =>
		{
			if (!_debugInformation.Visible) return;
			UpdateObjects();
			UpdateScene();
		};
		_debugInformation.Visible = false;
		UpdateStaticLabels();
	}
	
	private static string ConvertToMemoryFormat(ulong mem)
	{
		return mem switch
		{
			>= 0x40000000 => (float)Math.Round(mem / 1024f / 1024f / 1024f, 2) + " GB",
			>= 0x100000 => (float)Math.Round(mem / 1024f / 1024f, 2) + " MB",
			>= 0x400 => (float)Math.Round(mem / 1024f, 2) + " KB",
			_ => mem + " B"
		};
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("debug_toggle")) 
			_debugInformation.Visible = !_debugInformation.Visible;
		
		UpdateFps();
		
		_ramUpdateTime += (float)delta;
		if (_ramUpdateTime >= 1f)
		{
			UpdateRam();
			if (_vram.Visible) UpdateVram();
			_ramUpdateTime = 0f;
		}

		if (!_debugInformation.Visible) return;
		
		_objectUpdateTime += (float)delta;
		if (_objectUpdateTime >= 2f)
		{
			UpdateObjects();
			UpdateScene();
			_currentScene.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";
			_objectUpdateTime = 0f;
		}

		if (Conductor.Singleton != null)
		{
			UpdateConductor();
			_conductorInfo.Visible = true;
		}
		else _conductorInfo.Visible = false;
	}

	private string GetKeybinds(Node node) => string.Join(", ", InputMap.ActionGetEvents(node.Name).OfType<InputEventKey>().Select(key => key.AsTextPhysicalKeycode()));

	private void UpdateStaticLabels()
	{
		_gameVersion.Text = $"{ProjectSettings.GetSetting("application/config/name").AsString()} {ProjectSettings.GetSetting("application/config/version").AsString()} {(OS.IsDebugBuild() ? "[Debug]" : "[Release]")}";
		_rubiconVersion.Text = $"Rubicon Engine {RubiconEngine.GetVersion()}";
		_godotVersion.Text = $"Godot Engine {Engine.GetVersionInfo()["major"]}.{Engine.GetVersionInfo()["minor"]}.{Engine.GetVersionInfo()["patch"]} [{Engine.GetVersionInfo()["status"]}]";
	}

	private void UpdateFps() => _fps.Text = $"FPS: {Mathf.FloorToInt(1 / GetProcessDeltaTime())}";

	private void UpdateRam() => _ram.Text = $"RAM: {ConvertToMemoryFormat(OS.GetStaticMemoryUsage())} [{ConvertToMemoryFormat(OS.GetStaticMemoryPeakUsage())}]";

	private void UpdateVram() => _vram.Text = $"VRAM: {ConvertToMemoryFormat((ulong)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed))}";

	private void UpdateScene() => _currentScene.Text = $"Scene: {(GetTree().CurrentScene != null && GetTree().CurrentScene.SceneFilePath != "" ? GetTree().CurrentScene.SceneFilePath : "None")}";

	private void UpdateObjects()
	{
		_allObjects.Text = $"All Instantiated Objects: {Performance.GetMonitor(Performance.Monitor.ObjectCount)}";
		_resourceObjects.Text = $"Resource Objects in Use: {Performance.GetMonitor(Performance.Monitor.ObjectResourceCount)}";
		_nodeObjects.Text = $"Node Objects: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)} (Orphan Nodes: {Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount)})";
	}

	private readonly StringBuilder ConductorSB = new();

	private void UpdateConductor()
	{
		ConductorSB.Clear();

		ConductorSB.AppendLine($"BPM: {Conductor.Bpm}, Audio Position: {Conductor.RawTime}")
			.AppendLine($"Step: {Conductor.CurrentStep}")
			.AppendLine($"Beat: {Conductor.CurrentBeat}")
			.AppendLine($"Measure: {Conductor.CurrentMeasure}");;

		foreach (BpmInfo bpm in Conductor.BpmList)
			ConductorSB.AppendLine($"Time: {bpm.Time}, Exact Time (ms): {bpm.MsTime}, BPM: {bpm.Bpm}, Time Signature: {bpm.TimeSignatureNumerator}/{bpm.TimeSignatureDenominator}");

		_conductorInfo.Text = ConductorSB.ToString();
	}
}
