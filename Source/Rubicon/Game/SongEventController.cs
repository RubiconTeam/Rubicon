using System.Collections.Generic;
using Rubicon.Core;
using Rubicon.Core.Events;

namespace Rubicon.Game;

[GlobalClass] public partial class SongEventController : Node
{
    [Export] public int Index = 0;
    
    [Signal] public delegate void EventCalledEventHandler(StringName eventName, float time, Godot.Collections.Dictionary<StringName, Variant> args);

    [Export] private EventData[] _events = [];
    
    public void Setup(EventData[] events)
    {
        _events = events;
        
        List<StringName> eventsInitialized = new List<StringName>();
        for (int i = 0; i < events.Length; i++)
        {
            if (eventsInitialized.Contains(events[i].Name))
                return;
            
            eventsInitialized.Add(events[i].Name);
            
            // TODO: Actually initialize events
            // GD LOAD FOR NOW CUZ IM LAZY
            PackedScene eventScene = GD.Load<PackedScene>($"res://Resources/Events/{events[i].Name}.tscn");
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