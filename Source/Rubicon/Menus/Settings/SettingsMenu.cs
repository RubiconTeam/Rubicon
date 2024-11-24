using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rubicon.Menus;
using Rubicon.Data.Settings.Attributes;
using Rubicon.Menus.Settings;

namespace Rubicon.Data.Settings;
public partial class SettingsMenu : BaseMenu
{
	private const string DefaultIconPath = "res://Assets/UI/Menus/Settings/Gameplay.png";

	[Export] private VBoxContainer _sectionButtonContainer;
	[Export] private ScrollContainer _settingsSectionContainer;

	#region Templates
	/* Section Template */
	[Export] private Button _sectionButtonTemplate;

	/* Section Setting Container Template */
	[Export] private VBoxContainer _sectionContainerTemplate;
	
	/* Group Separator Template */
	[Export] private VBoxContainer _groupSeparatorTemplate;

	/* Setting Button Templates */
	[Export] private Button _buttonTemplate;
	[Export] private CheckButton _checkButtonTemplate;
	[Export] private MenuButton _dropDownTemplate;

	/* ColorPickerButton (Parent.Picker) */
	[Export] private Label _colorPickerTemplate;

	/* LineEdit (_lineEditTemplate.LineEdit) */
	[Export] private Label _lineEditTemplate;

	/* HSlider (_sliderTemplate.Slider) */
	/* Label (_sliderTemplate.Slider.Min) */
	/* Label (_sliderTemplate.Slider.Max) */
	[Export] private Label _sliderTemplate;
	
	/* Button (_sliderTemplate.CurrentKeybind) */
	[Export] private Label _keybindTemplate;
	#endregion

	private Dictionary<Button, VBoxContainer> _sectionMapping = new();

	public override void _Ready()
	{
		base._Ready();

		foreach (Control control in new Control[]
				 {
					 _sectionButtonTemplate,
					 _buttonTemplate,
					 _checkButtonTemplate,
					 _dropDownTemplate,
					 _colorPickerTemplate,
					 _sliderTemplate,
					 _lineEditTemplate,
					 _sectionContainerTemplate
				 })
			control.Visible = false;

		GenerateSections();
		SelectFirstSection();
	}

	private void GenerateSections()
	{
		foreach (PropertyInfo prop in typeof(UserSettingsInstance).GetProperties())
		{
			RubiconSettingsSectionAttribute attribute = prop.PropertyType.GetCustomAttribute<RubiconSettingsSectionAttribute>();
			if (attribute != null && attribute.GenerateInMenu)
			{
				Texture2D icon = attribute.Icon ?? new Texture2D() { ResourcePath = DefaultIconPath };
				Button sectionButton = SettingsFactory.CreateSectionButton(_sectionButtonTemplate, attribute.Name, icon);
				if (sectionButton != null)
				{
					_sectionButtonContainer.AddChild(sectionButton);
					sectionButton.Pressed += () => SelectSection(sectionButton);
				}

				VBoxContainer sectionContainer = SettingsFactory.CreateSectionContainer(_sectionContainerTemplate, attribute.Name, attribute.GenerateInMenu);
				if (sectionContainer != null)
				{
					_settingsSectionContainer.AddChild(sectionContainer);
					_sectionMapping[sectionButton ?? throw new InvalidOperationException()] = sectionContainer;
				}
			}
		}
	}

	private void SelectFirstSection()
	{
		if (_sectionMapping.Count > 0)
		{
			Button firstButton = _sectionMapping.Keys.First();
			SelectSection(firstButton);
		}
	}

	private void SelectSection(Button selectedButton)
	{
		foreach (var entry in _sectionMapping)
		{
			Button button = entry.Key;
			VBoxContainer container = entry.Value;

			button.ButtonPressed = (button == selectedButton);
			container.Visible = (button == selectedButton);
		}
	}
}
