using Godot.Collections;
using Rubicon.Core.API;

// This is a template for events you place in the chart editor.
// This can also act as a Node! So yes, you will have access to such things like _Process(delta).
public partial class NewSongEvent : CsSongEvent
{
	// This is basically your _Ready() function.
	// Do note that you can access the PlayField just by getting "PlayField"!
	public override void Initialize()
	{
		
	}

	// Called when the event controller reaches this event.
	public override void CallEvent(float time, Dictionary<StringName, Variant> args)
	{
		
	}
}
