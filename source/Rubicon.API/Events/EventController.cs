using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Promise.Framework;
using Promise.Framework.Utilities;
using Rubicon.Data.Events;
using Rubicon.Data.Meta;

namespace Rubicon.API.Events;

public partial class EventController : Node
{
    [Export] public int EventTriggerIndex = 0;

    [Export] public ChartEvents EventData;
    
    public List<ISongEvent> Events;

    public void Load(ChartEvents data)
    {
        EventData = data;
        Events = new List<ISongEvent>();

        Type[] eventTypes = AppDomain.CurrentDomain.GetTypesWithInterface<ISongEvent>();
        for (int i = 0; i < eventTypes.Length; i++)
        {
            ISongEvent songEvent = (ISongEvent)eventTypes[i].GetConstructor(new Type[] { }).Invoke(new object[] { });
            EventData[] matchingEvents = EventData.Events.Where(x => x.Name == songEvent.Name).ToArray();
            if (matchingEvents.Length > 0)
            {
                Events.Add(songEvent);
                    
                for (int j = 0; j < matchingEvents.Length; j++)
                    songEvent.OnReady(matchingEvents[j]);
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if (!Conductor.Playing || EventData == null || EventTriggerIndex >= EventData.Events.Length)
            return;

        EventData curEvent = EventData.Events[EventTriggerIndex];
        if (Conductor.Time * 1000d >= curEvent.MsTime)
        {
            ISongEvent songEvent = Events.FirstOrDefault(x => x.Name == curEvent.Name);
            if (songEvent != null)
                songEvent.OnTrigger(curEvent.Arguments);
            else
                GD.PrintErr($"Tried to run event \"{curEvent.Name}\" at measure {curEvent.Time}, but no song event was found!");

            EventTriggerIndex++;
        }
    }

    public override void _ExitTree()
    {
        foreach (ISongEvent songEvent in Events)
            songEvent?.OnFree();

        base._ExitTree();
    }
}