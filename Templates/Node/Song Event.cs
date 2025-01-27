using Godot.Collections;
using Rubicon.API;

// This can also act as a Node! So yes, you will have access to such things like _Ready() and _Process(delta).
public partial class NewSongEvent : CSSongEvent
{
	// Called when the event controller reaches this event.
	public override void CallEvent(float time, Dictionary<StringName, Variant> args)
	{
		
	}
}
