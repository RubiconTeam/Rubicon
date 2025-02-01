using Godot.Collections;
using Rubicon.API;
using Rubicon.Core.API;
using Rubicon.Core.Data;
using Rubicon.Data;
using Rubicon.Game;
using Rubicon.View2D;

namespace Rubicon.Extras.Events;

// This is a template for events you place in the chart editor.
// This can also act as a Node! So yes, you will have access to such things like _Ready() and _Process(delta).
[GlobalClass] public partial class SetCameraFocusSongEvent : CsSongEvent
{
	// Called when the event controller reaches this event.
	public override void CallEvent(float time, Dictionary<StringName, Variant> args)
	{
		#if FUNKIN
		StringName focusKey = new StringName("Focus");
		if (!args.ContainsKey(focusKey))
			return;
		
		StringName focusOn = args[focusKey].AsStringName();
		RubiconGameInstance gameInstance = RubiconGame.Singleton;
		switch (gameInstance.Metadata.Environment)
		{
			case GameEnvironment.CanvasItem:
				CanvasItemSpace space = gameInstance.CanvasItemSpace;
				space.Camera.TargetPosition = space.GetGroupCameraPosition(focusOn);
				break;
		}
		#endif
	}
}

