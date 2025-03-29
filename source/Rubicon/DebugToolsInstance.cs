using System.Linq;
using PukiTools.GodotSharp;
using PukiTools.GodotSharp.Screens;

namespace Rubicon;

[Autoload("DebugTools"), GlobalClass] public partial class DebugToolsInstance : Control
{
    public enum DebugVisibility
    {
        Hidden,
        FpsOnly,
        Everything
    }

    [Export]
    public DebugVisibility Visiblity
    {
        get => _visibility;
        set
        {
            _visibility = value;
            ChangeVisibility(_visibility);
        }
    }

    [Export] public Label FpsLabel;
    
    [Export] public Control EverythingContainer;
    [Export] public Label GameVersion;
    [Export] public Label RubiconVersion;
    [Export] public Label GodotVersion;
    
    [ExportSubgroup("Debug Only"), Export] public Control DebugOnlyContainer;
    [Export] public Label RamLabel;
    [Export] public Label VRamLabel;
    
    [Export] public Label AllObjects;
    [Export] public Label NodeObjects;
    [Export] public Label ResourceObjects;

    [Export] public Control ScreenContainer;
    [Export] public Label CurrentScreen;
    [Export] public Label ScreenInfo;

    private int[] _fps = new int[60];
    private int _fpsIndex = 0;
    
    private DebugVisibility _visibility = DebugVisibility.Hidden;
    
    public override void _Ready()
    {
        base._Ready();
        
        for (int i = 0; i < _fps.Length; i++) 
            _fps[i] = 0;
        
        GameVersion.Text = $"{ProjectSettings.GetSetting("application/config/name").AsString()} {ProjectSettings.GetSetting("application/config/version").AsString()} [{(Engine.IsEditorHint() ? "Editor" : OS.IsDebugBuild() ? "Debug" : "Release")}]";
        RubiconVersion.Text = $"Rubicon Engine {RubiconEngine.GetVersion()}";
        GodotVersion.Text = $"Godot Engine {Engine.GetVersionInfo()["major"]}.{Engine.GetVersionInfo()["minor"]}.{Engine.GetVersionInfo()["patch"]} [{Engine.GetVersionInfo()["status"]}]";

        DebugOnlyContainer.Visible = OS.IsDebugBuild();
        ChangeVisibility(_visibility);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        _fps[_fpsIndex] = Mathf.FloorToInt(1d / delta);
        _fpsIndex = (_fpsIndex + 1) % _fps.Length;

        if (_visibility > DebugVisibility.Hidden)
            FpsLabel.Text = Mathf.FloorToInt(_fps.Average()) + " FPS";

        if (_visibility < DebugVisibility.Everything || !OS.IsDebugBuild())
            return;
        
        RamLabel.Text = $"RAM: {OS.GetStaticMemoryUsage().ToMemoryFormat()} / {OS.GetStaticMemoryPeakUsage().ToMemoryFormat()}";
        VRamLabel.Text = $"VRAM: {((ulong)Performance.GetMonitor(Performance.Monitor.RenderTextureMemUsed)).ToMemoryFormat()}";

        AllObjects.Text = $"Instantiated Object Count: {Performance.GetMonitor(Performance.Monitor.ObjectCount)}";
        ResourceObjects.Text = $"Resource Count: {Performance.GetMonitor(Performance.Monitor.ObjectResourceCount)}";
        NodeObjects.Text = $"Node Count: {Performance.GetMonitor(Performance.Monitor.ObjectNodeCount)} (Orphaned: {Performance.GetMonitor(Performance.Monitor.ObjectOrphanNodeCount)})";

        ScreenContainer.Visible = ScreenManager.CurrentScreen != null;
        if (ScreenManager.CurrentScreen == null)
            return;
        
        CurrentScreen.Text = $"Scene Path: {ScreenManager.CurrentScreen.SceneFilePath}";

        string screenInfo = ScreenManager.GetCurrentScreenDebugInfo();
        ScreenInfo.Text = string.IsNullOrWhiteSpace(screenInfo) ? "No info to display." : screenInfo;
    }

    public void ChangeVisibility(DebugVisibility visibility)
    {
        bool fpsVisible = visibility > DebugVisibility.Hidden;
        bool everythingVisible = visibility == DebugVisibility.Everything;

        FpsLabel.Visible = fpsVisible;
        EverythingContainer.Visible = everythingVisible;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsEcho())
            return;

        if (!@event.IsActionPressed("debug_toggle"))
            return;

        int vis = (int)_visibility;
        vis = (vis + 1) % 3;
        _visibility = (DebugVisibility)vis;
        
        ChangeVisibility(_visibility);
    }
}