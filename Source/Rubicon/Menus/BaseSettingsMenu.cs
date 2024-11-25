using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rubicon.Menus.Settings;
public static class ControlExtensions
{
	public static T Configure<T>(this T control, Action<T> configure) where T : Control
	{
		configure(control);
		return control;
	}
}

public abstract partial class BaseSettingsMenu : BaseMenu
{
	#region Template Fields
	[ExportGroup("Templates")]
	[Export] private PackedScene _sectionButtonTemplate;
	[Export] private PackedScene _sectionContainerTemplate;
	[Export] private PackedScene _groupSeparatorTemplate;
	[Export] private PackedScene _buttonTemplate;
	[Export] private PackedScene _checkButtonTemplate;
	[Export] private PackedScene _dropDownTemplate;
	[Export] private PackedScene _colorPickerTemplate;
	[Export] private PackedScene _lineEditTemplate;
	[Export] private PackedScene _sliderTemplate;
	[Export] private PackedScene _keybindTemplate;
	#endregion

	#region Factory Methods
	private protected Button CreateSectionButton(string sectionName, Texture2D icon, bool generateInMenu = true)
	{
		if (!generateInMenu) return null;

		return InstantiateTemplate<Button>(_sectionButtonTemplate, "Section Button Template is not a Button / is null.")
			.Configure(button =>
			{
				button.Name = sectionName;
				button.Text = sectionName;
				button.Icon = icon;
				button.Visible = true;
			});
	}

	private protected VBoxContainer CreateSectionContainer(string sectionName, bool generateInMenu = true)
	{
		if (!generateInMenu) return null;

		return InstantiateTemplate<VBoxContainer>(_sectionContainerTemplate, "Section Container Template is not a VBoxContainer / is null.")
			.Configure(container => container.Name = $"{sectionName}Container");
	}

	private protected MenuButton CreateDropdownMenu(string name, IEnumerable<string> options)
	{
		var menuButton = InstantiateTemplate<MenuButton>(_dropDownTemplate, "Template is not a MenuButton / is null.")
			.Configure(button =>
			{
				button.Name = name;
				button.Text = name;
			});

		var popup = menuButton.GetPopup();
		foreach (var option in options)
		{
			popup.AddItem(option);
		}

		return menuButton;
	}

	private protected Label CreateSeparator(string text)
	{
		return InstantiateTemplate<Label>(_groupSeparatorTemplate, "Template is not a Label / is null.")
			.Configure(separator =>
			{
				separator.Name = $"{text}_Separator";
				separator.Text = text;
			});
	}

	private protected ColorPickerButton CreateColorPickerButton(string name, Color initialColor)
	{
		return InstantiateTemplate<ColorPickerButton>(_colorPickerTemplate, "Template is not a ColorPickerButton / is null.")
			.Configure(picker =>
			{
				picker.Name = name;
				picker.Color = initialColor;
			});
	}

	private protected Button CreateCheckButton(string name, bool isChecked)
	{
		return InstantiateTemplate<Button>(_checkButtonTemplate, "Template is not a Button / is null.")
			.Configure(button =>
			{
				button.Name = name;
				button.Text = name;
				button.ButtonPressed = isChecked;
				button.ToggleMode = true;
			});
	}

	private protected LineEdit CreateLineEdit(string name, string initialText = "", string placeholder = "")
	{
		return InstantiateTemplate<LineEdit>(_lineEditTemplate, "Template is not a LineEdit / is null.")
			.Configure(lineEdit =>
			{
				lineEdit.Name = name;
				lineEdit.Text = initialText;
				lineEdit.PlaceholderText = placeholder;
			});
	}

	private protected Button CreateKeybindButton(string name, string currentKeybind)
	{
		return InstantiateTemplate<Button>(_keybindTemplate, "Template is not a Button / is null.")
			.Configure(button =>
			{
				button.Name = name;
				button.Text = currentKeybind;
			});
	}

	private protected MenuButton CreateDropdownMenuFromEnum<TEnum>(string name) where TEnum : struct, Enum
	{
		var options = Enum.GetValues<TEnum>().Select(e => e.ToString());
		return CreateDropdownMenu(name, options);
	}

	private protected Button CreateToggleButton(string name, bool isActive)
	{
		return InstantiateTemplate<Button>(_buttonTemplate, "Template is not a Button / is null.")
			.Configure(button =>
			{
				button.Name = name;
				button.Text = name;
				button.ButtonPressed = isActive;
				button.ToggleMode = true;
			});
	}

	private protected (HSlider slider, Label minLabel, Label maxLabel) CreateSlider(string name, float minValue, float maxValue, float step, float initialValue = float.NaN)
	{
		var slider = InstantiateTemplate<HSlider>(_sliderTemplate, "Template is not an HSlider / is null.")
			.Configure(s =>
			{
				s.Name = name;
				s.MinValue = minValue;
				s.MaxValue = maxValue;
				s.Step = step;
				if (!float.IsNaN(initialValue))
				{
					s.Value = Math.Clamp(initialValue, minValue, maxValue);
				}
			});

		var minLabel = new Label { Text = minValue.ToString(CultureInfo.InvariantCulture) };
		var maxLabel = new Label { Text = maxValue.ToString(CultureInfo.InvariantCulture) };

		return (slider, minLabel, maxLabel);
	}
	#endregion

	#region Helper Methods
	private static T InstantiateTemplate<T>(PackedScene template, string errorMessage) where T : Control
	{
		if (template == null)
			throw new ArgumentNullException(nameof(template), "The provided template is null.");

		if (template.Instantiate() is not T instance)
			throw new InvalidOperationException($"{errorMessage} Expected: {typeof(T).Name}");

		return instance;
	}
	#endregion
}
