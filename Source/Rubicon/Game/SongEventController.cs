using System.Collections.Generic;
using Rubicon.Core;
using Rubicon.Core.Events;

namespace Rubicon.Game;

[GlobalClass] public partial class SongEventController : Node
{
    [Export] public int Index = 0;
    
    [Signal] public delegate void EventCalledEventHandler(StringName eventName, float time, Godot.Collections.Dictionary<StringName, Variant> args);

    [Export] private EventData[] _events = [];
    
    public void Setup(EventMeta eventMeta)
    {
        _events = eventMeta.Events;
        List<StringName> eventsInitialized = new List<StringName>();
        for (int i = 0; i < _events.Length; i++)
        {
            if (eventsInitialized.Contains(_events[i].Name))
                return;
            
            eventsInitialized.Add(_events[i].Name);

            string eventPath = $"res://Resources/Events/{_events[i].Name}";
            bool eventTscnExists = ResourceLoader.Exists(eventPath + ".tscn");
            bool eventScnExists = ResourceLoader.Exists(eventPath + ".scn");
            if (!eventTscnExists && !eventScnExists)
                continue;
            
            if (eventTscnExists)
                eventPath += ".tscn";
            else
                eventPath += ".scn";
            
            PackedScene eventScene = GD.Load<PackedScene>(eventPath);
            AddChild(eventScene.Instantiate());
        }
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Index >= _events.Length)
            return;
        
        EventData curEvent = _events[Index];
        if (Conductor.Time * 1000f >= curEvent.MsTime)
        {
            EmitSignalEventCalled(curEvent.Name, curEvent.Time, curEvent.Arguments);
            Index++;
        }
    }
}