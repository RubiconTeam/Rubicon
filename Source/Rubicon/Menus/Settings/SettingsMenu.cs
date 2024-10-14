using Godot;
using System;
using System.Reflection;
using Rubicon.Menus;
using Rubicon.Data.Settings;
using Rubicon.Data.Settings.Attributes;

public partial class SettingsMenu : BaseMenu
{
	[NodePath("Main/Sidebar/AnimationPlayer")] private AnimationPlayer _sidebarAnimationPlayer;
	[NodePath("Main/Sidebar")] private Control _sidebar;
	[NodePath("Main/Sidebar/BG/Container")] private Control _buttonContainer;
	[NodePath("Main/Sidebar/BG/Labels")] private Control _labelsParent;

	private const string ButtonTemplatePath = "res://Source/Rubicon.Menus/Settings/Resources/SectionButtonTemplate.tscn";
	private const string DefaultIconPath = "res://Assets/UI/Menus/Settings/Gameplay.png";

	public override void _Ready()
	{
		this.OnReady();
		base._Ready();
		
		_sidebar.MouseEntered += () => _sidebarAnimationPlayer.Play("Enter");
		_sidebar.MouseExited += () => _sidebarAnimationPlayer.Play("Leave");

		foreach (var prop in typeof(UserSettingsData).GetProperties())
		{
			var attribute = prop.PropertyType.GetCustomAttribute<RubiconSettingsSectionAttribute>();
			if (attribute != null && attribute.GenerateInMenu) CreateSectionButton(attribute.Name, attribute.Icon);
		}
	}
	
	private void CreateSectionButton(string sectionName, Texture2D icon)
	{
		var buttonInstance = GD.Load<PackedScene>(ButtonTemplatePath).Instantiate<Control>();
		buttonInstance.Name = sectionName;

		var textureRect = buttonInstance.GetNode<TextureRect>("Icon");
		var label = buttonInstance.GetNode<Label>("Text");

		textureRect.Texture = icon;
		label.Text = sectionName;
		label.Name = sectionName;

		_buttonContainer.AddChild(buttonInstance);
		label.GetParent().RemoveChild(label);
		_labelsParent.AddChild(label);
		CallDeferred(nameof(PositionLabel), label, buttonInstance);
	}

	private void PositionLabel(Label label, Control button) => label.Position = new Vector2(30, ((_buttonContainer.GetGlobalTransformWithCanvas().AffineInverse() * button.GetGlobalTransformWithCanvas().Origin).Y + button.Size.Y / 2) - label.Size.Y / 2);
}
