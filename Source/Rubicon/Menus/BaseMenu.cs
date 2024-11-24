namespace Rubicon.Menus;

public abstract partial class BaseMenu : Node
{
	[ExportGroup("Status")] 
	[Export] public int Selection = 0;
		
	[ExportGroup("Settings")] 
	[Export] public bool AllowScrollWheel = true;
	[Export] public bool AllowEcho = true;
		
	[ExportGroup("References")] 
	[Export] public AudioStream MoveCursor;
	[Export] public AudioStream ConfirmAudio;
	[Export] public AudioStream BackAudio;

	public virtual void DownPressed(bool isPressed) {}
	public virtual void UpPressed(bool isPressed) {}
	public virtual void LeftPressed(bool isPressed) {}
	public virtual void RightPressed(bool isPressed) {}
	public virtual void ConfirmPressed(bool isPressed) {}
	public virtual void BackPressed(bool isPressed) {}
	public virtual void Scroll(float direction) {}

	public virtual void UpdateSelection() {}

	public override void _Ready() => UpdateSelection();

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event.IsAction("ui_down", AllowEcho))
			DownPressed(@event.IsPressed());
		else if (@event.IsAction("ui_up", AllowEcho)) 
			UpPressed(@event.IsPressed());
		else if (@event.IsAction("ui_left", AllowEcho))
			LeftPressed(@event.IsPressed());
		else if (@event.IsAction("ui_right", AllowEcho))
			RightPressed(@event.IsPressed());
		else if (@event.IsAction("ui_accept"))
			ConfirmPressed(@event.IsPressed());
		else if (@event.IsAction("ui_cancel", AllowEcho))
			BackPressed(@event.IsPressed());

		if (!AllowScrollWheel)
			return;

		if (@event is InputEventMouseButton mouseEvent)
		{
			switch (mouseEvent.ButtonIndex)
			{
				case MouseButton.WheelDown:
					Scroll(mouseEvent.Factor);
					break;
				case MouseButton.WheelUp:
					Scroll(mouseEvent.Factor);
					break;
			}
		}
	}
}
