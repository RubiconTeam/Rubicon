using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rubicon.Data.Settings;
using Rubicon.Menus;
using Rubicon.Data.Settings.Attributes;
using Rubicon.Menus.Settings;

namespace Rubicon.Menus.Settings;
public partial class SettingsMenu : BaseSettingsMenu
{
	private const string DefaultIconPath = "res://Assets/UI/Menus/Settings/Gameplay.png";

	[Export] private VBoxContainer _sectionButtonContainer;
	[Export] private ScrollContainer _settingsSectionContainer;
	[Export] private Label _settingsDescriptionLabel;
	
	private readonly Dictionary<Button, VBoxContainer> _sectionMapping = new();

	public override void _Ready()
	{
		base._Ready();
		GenerateSections();
		
		if (_sectionMapping.Count > 0)
		{
			Button firstButton = _sectionMapping.Keys.First();
			SelectSection(firstButton);
		}
	}

	private void GenerateSections()
	{
		foreach (PropertyInfo prop in typeof(UserSettingsInstance).GetProperties())
		{
			RubiconSettingsSectionAttribute attribute = prop.PropertyType.GetCustomAttribute<RubiconSettingsSectionAttribute>();
			if (attribute == null) continue;
			
			Texture2D icon = attribute.Icon ?? new Texture2D() { ResourcePath = DefaultIconPath };
			Button sectionButton = CreateSectionButton(attribute.Name, icon, attribute.GenerateInMenu);
			if (sectionButton != null)
			{
				_sectionButtonContainer.AddChild(sectionButton);
				sectionButton.Pressed += () => SelectSection(sectionButton);
			}

			VBoxContainer sectionContainer = CreateSectionContainer(attribute.Name, attribute.GenerateInMenu);
			if (sectionContainer != null)
			{
				_settingsSectionContainer.AddChild(sectionContainer);
				_sectionMapping[sectionButton ?? throw new InvalidOperationException()] = sectionContainer;
			}
		}
	}

	private void SelectSection(Button selectedButton)
	{
		foreach (var (button, container) in _sectionMapping)
		{
			button.ButtonPressed = (button == selectedButton);
			container.Visible = (button == selectedButton);
		}
	}
}
