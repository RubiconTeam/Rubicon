using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rubicon.Menus.Settings;
public abstract partial class BaseSettingsMenu : BaseMenu
{
	/* Section Template */
	[ExportGroup("Templates")] 
	[Export] private PackedScene _sectionButtonTemplate;

	/* Section Setting Container Template */
	[Export] private PackedScene _sectionContainerTemplate;
	
	/* Group Separator Template */
	[Export] private PackedScene _groupSeparatorTemplate;

	/* Setting Button Templates */
	[Export] private PackedScene _buttonTemplate;
	[Export] private PackedScene _checkButtonTemplate;
	[Export] private PackedScene _dropDownTemplate;

	/* ColorPickerButton (Parent.Picker) */
	[Export] private PackedScene _colorPickerTemplate;

	/* LineEdit (_lineEditTemplate.LineEdit) */
	[Export] private PackedScene _lineEditTemplate;

	/* HSlider (_sliderTemplate.Slider) */
	/* Label (_sliderTemplate.Slider.Min) */
	/* Label (_sliderTemplate.Slider.Max) */
	[Export] private PackedScene _sliderTemplate;
	
	/* Button (_sliderTemplate.CurrentKeybind) */
	[Export] private PackedScene _keybindTemplate;
	
	public Button CreateSectionButton(string sectionName, Texture2D icon, bool generateInMenu)
	{
		if (!generateInMenu)
			return null;

		var buttonInstance = InstantiateTemplate<Button>(_sectionButtonTemplate, "Section Button Template is not a Button / is null.");
		buttonInstance.Name = sectionName;
		buttonInstance.Text = sectionName;
		buttonInstance.Icon = icon;
		buttonInstance.Visible = true;

		return buttonInstance;
	}
	
	public VBoxContainer CreateSectionContainer(string sectionName, bool generateInMenu)
	{
		if (!generateInMenu)
			return null;

		var containerInstance = InstantiateTemplate<VBoxContainer>(_sectionContainerTemplate, "Section Container Template is not a VBoxContainer / is null.");
		containerInstance.Name = $"{sectionName}Container";
		return containerInstance;
	}

	public MenuButton CreateDropdownMenu(string name, IEnumerable<string> options)
	{
		var menuButton = InstantiateTemplate<MenuButton>(_dropDownTemplate, "Template is not a MenuButton / is null.");
		menuButton.Name = name;
		menuButton.Text = name;

		foreach (var option in options)
		{
			menuButton.GetPopup().AddItem(option);
		}

		return menuButton;
	}
	
	public Label CreateSeparator(string text)
	{
		var separator = InstantiateTemplate<Label>(_groupSeparatorTemplate, "Template is not a Label / is null.");
		separator.Name = $"{text}_Separator";
		separator.Text = text;
		return separator;
	}

	public ColorPickerButton CreateColorPickerButton(string name, Color initialColor)
	{
		var colorPicker = InstantiateTemplate<ColorPickerButton>(_colorPickerTemplate, "Template is not a ColorPickerButton / is null.");
		colorPicker.Name = name;
		colorPicker.Color = initialColor;
		return colorPicker;
	}

	public MenuButton CreateDropdownMenuFromEnum<TEnum>(string name) where TEnum : struct, Enum
	{
		var options = Enum.GetValues<TEnum>().Select(e => e.ToString());
		return CreateDropdownMenu(name, options);
	}
	
	public Button CreateToggleButton(string name, bool isActive)
	{
		var button = InstantiateTemplate<Button>(_buttonTemplate, "Template is not a Button / is null.");
		button.Name = name;
		button.Text = name;
		button.ButtonPressed = isActive;
		return button;
	}

	public (HSlider slider, Label minLabel, Label maxLabel) CreateSlider(float minValue, float maxValue, float step)
	{
		var slider = InstantiateTemplate<HSlider>(_sliderTemplate, "Template is not an HSlider / is null.");
		slider.MinValue = minValue;
		slider.MaxValue = maxValue;
		slider.Step = step;

		var minLabel = new Label { Text = minValue.ToString(CultureInfo.InvariantCulture) };
		var maxLabel = new Label { Text = maxValue.ToString(CultureInfo.InvariantCulture) };

		return (slider, minLabel, maxLabel);
	}

	private static T InstantiateTemplate<T>(PackedScene template, string errorMessage) where T : class
	{
		if (template == null)
			throw new ArgumentNullException(nameof(template), "The provided template is null.");

		if (template.Instantiate() is not T instance)
			throw new InvalidOperationException($"{errorMessage} Expected: {typeof(T).Name}");

		return instance;
	}
}
