using System.Linq;
using Rubicon.Core;
using Array = Godot.Collections.Array;
using ThreadLoadStatus = Godot.ResourceLoader.ThreadLoadStatus;

namespace Rubicon.Screens;

/// <summary>
/// Handles switching screens as well as preloading the next screen's assets.
/// </summary>
[GlobalClass, StaticAutoloadSingleton("Rubicon.Screens", "ScreenManager")]
public partial class ScreenManagerInstance : CanvasLayer
{
    /// <summary>
    /// The current screen loaded.
    /// </summary>
    [Export] public Node CurrentScreen;
    
    /// <summary>
    /// The current loading screen.
    /// </summary>
    [Export] public Node LoadingScreen;

    /// <summary>
    /// How much progress has been done on loading.
    /// </summary>
    [Export] public int Progress = 0;
    
    /// <summary>
    /// Emits when the progress is updated.
    /// </summary>
    [Signal] public delegate void ProgressUpdatedEventHandler(int progress);
    
    /// <summary>
    /// Emits when loading is complete.
    /// </summary>
    [Signal] public delegate void CompletedEventHandler();
    
    private SceneTree _tree;

    private Array _progressArray = [];
    
    private string _screenPath;
    private bool _startLoading = false;
    private bool _screenLoaded = false;

    private ResourceLoadList _preloadList;
    private int _preloadIndex = 0;

    /// <summary>
    /// Sets up the node for use.
    /// </summary>
    public override void _Ready()
    {
        _tree = GetTree();
        Layer = 128;
    }

    /// <summary>
    /// Processes loading if there's a screen queued for loading.
    /// </summary>
    /// <param name="delta">Time passed, in seconds</param>
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (CurrentScreen == null)
            CurrentScreen = _tree.CurrentScene;

        if (_screenPath == null || !_startLoading)
            return;

        if (!_screenLoaded)
        {
            ThreadLoadStatus screenLoadStatus = ResourceLoader.LoadThreadedGetStatus(_screenPath, _progressArray);
            if (screenLoadStatus == ThreadLoadStatus.InProgress)
            {
                Progress = Mathf.FloorToInt(_progressArray[0].AsSingle() * 50f);
                EmitSignalProgressUpdated(Progress);
                return;
            }

            if (screenLoadStatus != ThreadLoadStatus.Loaded)
            {
                GD.PrintErr($"[ScreenManager] Failed to load screen at path {_screenPath}. Error: {screenLoadStatus}");
                Reset();
                return;
            }

            _screenLoaded = true;
            Resource screenResource = ResourceLoader.LoadThreadedGet(_screenPath);
            if (!screenResource.IsClass("PackedScene"))
            {
                GD.PrintErr($"[ScreenManager] Resource at {_screenPath} is not of type PackedScene.");
                Reset();
                return;
            }
                
            Node screen = (screenResource as PackedScene).Instantiate();
            if (screen is null)
            {
                GD.PrintErr($"[ScreenManager] Resource at {_screenPath} is not of type Screen.");
                Reset();
                return;
            }

            CurrentScreen = screen;
            Progress = 50;
            EmitSignalProgressUpdated(Progress);
            
            // Start preloading
            CallReadyPreload();
            UpdateResourcePaths();
            _preloadIndex = 0;
            
            if (_preloadList.Count != 0)
                ResourceLoader.LoadThreadedRequest(_preloadList[_preloadIndex]);
        }

        // Continue processing preloading here
        if (_preloadIndex < _preloadList.Count)
        {
            string currentPath = _preloadList[_preloadIndex];
            while (!ResourceLoader.Exists(currentPath))
            {
                _preloadIndex++;

                if (_preloadIndex >= _preloadList.Count)
                    break;
                
                currentPath = _preloadList[_preloadIndex];
            }
            
            ThreadLoadStatus resourceStatus = ResourceLoader.LoadThreadedGetStatus(currentPath, _progressArray);
            int progressOffset = 50 + Mathf.FloorToInt((float)_preloadIndex / _preloadList.Count * 50f);
            int progress = progressOffset;
            switch (resourceStatus)
            {
                case ThreadLoadStatus.InvalidResource: // Not loaded yet
                    ResourceLoader.LoadThreadedRequest(currentPath);
                    break;
                case ThreadLoadStatus.InProgress:
                    progress += Mathf.FloorToInt(1f / _preloadList.Count * _progressArray[0].AsSingle() * 50f);
                    break;
                case ThreadLoadStatus.Loaded:
                    NotifyResourceLoaded(currentPath);
                    UpdateResourcePaths();
                    _preloadIndex++;
                    
                    progress += Mathf.FloorToInt(1f / _preloadList.Count * _progressArray[0].AsSingle() * 50f);
                    break;
                default:
                    GD.PrintErr($"[ScreenManager] Failed to load resource at path {currentPath}. Error: {resourceStatus}");
                    _preloadIndex++;
                    break;
            }

            Progress = progress;
            EmitSignalProgressUpdated(Progress);
            
            return;
        }

        Progress = 100;
        
        _tree.Root.AddChild(CurrentScreen);
        _tree.CurrentScene = CurrentScreen;
        
        EmitSignalProgressUpdated(Progress);
        EmitSignalCompleted();
        
        Reset();
    }
    
    /// <summary>
    /// Switches the current main screen to the screen at the path provided.
    /// </summary>
    /// <param name="path">The path provided.</param>
    /// <param name="loadingScreen">The loading screen to play.</param>
    public void SwitchScreen(string path, string loadingScreen = null)
    {
        if (!ResourceLoader.Exists(path))
        {
            GD.PrintErr($"[SceneManager] Couldn't find screen at path {path}. Aborting.");
            return;
        }
        
        Reset();
        _screenPath = path;

        string tscnPath = $"res://Resources/UI/Loading/{loadingScreen}.tscn";
        string scnPath = $"res://Resources/UI/Loading/{loadingScreen}.scn";
        
        bool tscnExists = ResourceLoader.Exists(tscnPath);
        bool scnExists = ResourceLoader.Exists(scnPath);
        if (!tscnExists && !scnExists)
        {
            StartLoading();
            return;
        }
        
        PackedScene loadingScene = GD.Load<PackedScene>(tscnExists ? tscnPath : scnPath);
        if (loadingScene is null)
        {
            StartLoading();
            return;
        }

        LoadingScreen = loadingScene.InstantiateOrNull<Node>();
        if (LoadingScreen is null)
        {
            StartLoading();
            return;
        }
        
        AddChild(LoadingScreen);
    }

    /// <summary>
    /// Actually starts loading the queued screen.
    /// </summary>
    public void StartLoading()
    {
        ResourceLoader.LoadThreadedRequest(_screenPath);
        _startLoading = true;
        
        CurrentScreen.QueueFree();
    }

    private void Reset()
    {
        Progress = 0;
        _screenPath = null;
        _screenLoaded = false;
        _preloadIndex = 0;
        _startLoading = false;
        _preloadList = [];
    }

    private void CallReadyPreload()
    {
        if (CurrentScreen is CsScreen cSharpScreen)
        {
            cSharpScreen.ReadyPreload();
            return;
        }

        GDScript screenScript = CurrentScreen.GetScript().As<GDScript>();
        if (screenScript.GetGlobalName() != "GDScreen")
            return;

        CurrentScreen.Call("ready_preload");
    }
    
    private void NotifyResourceLoaded(string path)
    {
        if (CurrentScreen is CsScreen cSharpScreen)
        {
            cSharpScreen.OnPreload(path);
            return;
        }

        GDScript screenScript = CurrentScreen.GetScript().As<GDScript>();
        if (screenScript.GetGlobalName() != "GDScreen")
            return;

        CurrentScreen.Call("on_resource_loaded", path);
    }

    private void UpdateResourcePaths()
    {
        if (CurrentScreen is CsScreen cSharpScreen)
        {
            _preloadList = cSharpScreen.ResourcesToLoad;
            return;
        }

        GDScript screenScript = CurrentScreen.GetScript().As<GDScript>();
        if (screenScript.GetGlobalName() != "GDScreen")
            return;

        _preloadList = CurrentScreen.Get("resources_to_load").As<ResourceLoadList>();
    }
}