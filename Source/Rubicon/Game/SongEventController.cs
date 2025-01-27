using System.Collections.Generic;
using Rubicon.Core.Events;

namespace Rubicon.Game;

[GlobalClass] public partial class SongEventController : Node
{
    [Export] public int Index = 0;
    
    [Signal] public delegate void EventCalledEventHandler(StringName eventName, float time, Godot.Collections.Dictionary<StringName, Variant> args);

    public void Setup(EventData[] events)
    {
        List<StringName> _eventsInitialized = new List<StringName>();
        for (int i = 0; i < events.Length; i++)
        {
            if (_eventsInitialized.Contains(events[i].Name))
                return;
            
            _eventsInitialized.Add(events[i].Name);
            
            // TODO: Actually initialize events
        }
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        
    }
}