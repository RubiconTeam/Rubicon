using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rubicon.Data.Settings;
using Rubicon.Data.Settings.Attributes;

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
		foreach (FieldInfo sectionField in typeof(UserSettingsData).GetFields())
		{
			var sectionAttribute = sectionField.FieldType.GetCustomAttribute<RubiconSettingsSectionAttribute>();
			if (sectionAttribute == null) continue;

			var sectionInstance = sectionField.GetValue(UserSettingsInstance._data);
			if (sectionInstance == null) continue;

			Texture2D icon = sectionAttribute.Icon ?? GD.Load<Texture2D>(DefaultIconPath);
			Button sectionButton = CreateSectionButton(sectionAttribute.Name, icon, sectionAttribute.GenerateInMenu);
			
			if (sectionButton != null)
			{
				_sectionButtonContainer.AddChild(sectionButton);
				sectionButton.Pressed += () => SelectSection(sectionButton);
			}

			VBoxContainer sectionContainer = CreateSectionContainer(sectionAttribute.Name, sectionAttribute.GenerateInMenu);
			if (sectionContainer != null)
			{
				_settingsSectionContainer.AddChild(sectionContainer);
				_sectionMapping[sectionButton ?? throw new InvalidOperationException()] = sectionContainer;
				GenerateSectionControls(sectionInstance, sectionContainer);
			}
		}
	}

	private void GenerateSectionControls(object sectionInstance, VBoxContainer container)
	{
		Type sectionType = sectionInstance.GetType();

		foreach (FieldInfo groupField in sectionType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
		{
			var groupAttribute = groupField.GetCustomAttribute<RubiconSettingsGroupAttribute>();
			if (groupAttribute != null)
			{
				container.AddChild(CreateSeparator(groupAttribute.SectionName));

				var groupInstance = groupField.GetValue(sectionInstance);
				if (groupInstance != null) GenerateSectionControls(groupInstance, container);
				continue;
			}

			Control control = CreateControlForField(sectionInstance, groupField);
			if (control != null) container.AddChild(control);
		}
	}

	private Control CreateControlForField(object sectionInstance, FieldInfo field)
	{
		Type fieldType = field.FieldType;
		var stepValueAttribute = field.GetCustomAttribute<StepValueAttribute>();
		object currentValue = field.GetValue(sectionInstance);

		switch (currentValue)
		{
			case bool boolValue:
				return CreateCheckButton(field.Name, boolValue)
					.Configure(btn => 
					{
						btn.Toggled += (toggled) => 
							field.SetValue(sectionInstance, toggled);
					});

			case int intValue when stepValueAttribute != null:
				var (ilabel, iinstance) = CreateSlider(field.Name, stepValueAttribute.Minimum, stepValueAttribute.Maximum, stepValueAttribute.Step, (int)intValue);
				iinstance.ValueChanged += (val) => 
					field.SetValue(sectionInstance, (int)val);
				
				return ilabel;
			
			case float floatValue when stepValueAttribute != null:
				var (flabel, finstance) = CreateSlider(field.Name, stepValueAttribute.Minimum, stepValueAttribute.Maximum, stepValueAttribute.Step, (float)floatValue);
				finstance.ValueChanged += (val) => 
					field.SetValue(sectionInstance, (float)val);
				
				return flabel;

			case double doubleValue when stepValueAttribute != null:
				var (dlabel, dinstance) = CreateSlider(field.Name, stepValueAttribute.Minimum, stepValueAttribute.Maximum, stepValueAttribute.Step, (float)doubleValue);
				dinstance.ValueChanged += (val) => 
					field.SetValue(sectionInstance, val);
				
				return dlabel;

			case Enum when fieldType.IsEnum:
				var dropdown = CreateDropdownMenuFromEnum(fieldType);
				dropdown.GetPopup().IdPressed += (id) => 
				{
					var selectedValue = Enum.ToObject(fieldType, (int)id);
					field.SetValue(sectionInstance, selectedValue);
				};
				return dropdown;

			default:
				return CreateLineEdit(field.Name, currentValue?.ToString() ?? "")
					.Configure(edit => 
					{
						edit.GetNode<LineEdit>("LineEdit").TextChanged += (text) => 
							field.SetValue(sectionInstance, text);
					});
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
