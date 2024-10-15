using Rubicon.Menus;

public partial class TitleMenu : BaseMenu
{
	[Export] private AnimationPlayer buttonsAnimationPlayer;
	[ExportGroup("Title Menu Buttons")]
	[Export] private VBoxContainer buttonsContainer;
	private bool isDirty;

	public override void _Ready()
	{
		base._Ready();
		foreach (Button button in buttonsContainer.GetChildren<Button>()) 
			button.Pressed += () => ButtonPressed(button.Name);
	}

	private void ButtonPressed(string buttonName)
	{
		if (!isDirty) return;
			
		switch (buttonName)
		{
			case "StoryMode":
			case "Debug":
			case "Settings":
			{
				GetTree().ChangeSceneToFile($"res://Source/Rubicon/Menus/{buttonName}/{buttonName}Menu.tscn");
				break;
			}
			default:
			{
				GD.Print("no idea what this scene is bro");
				break;
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsPressed() && !isDirty)
		{
			buttonsAnimationPlayer.Play("Intro");
			isDirty = true;
		}
	}
}
