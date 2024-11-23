using System.Reflection;
using Rubicon.Menus;
using Rubicon.Data.Settings.Attributes;

namespace Rubicon.Data.Settings;
public partial class SettingsMenu : BaseMenu
{
	[Export] private Control _sectionButtonContainer;
	[Export] private Control _sectionButtonTemplate;
	private const string DefaultIconPath = "res://Assets/UI/Menus/Settings/Gameplay.png";

	public override void _Ready()
	{
		base._Ready();

		_sectionButtonTemplate.Visible = false;
		
		foreach (PropertyInfo prop in typeof(UserSettingsData).GetProperties())
		{
			RubiconSettingsSectionAttribute attribute = prop.PropertyType.GetCustomAttribute<RubiconSettingsSectionAttribute>();
			if (attribute != null && attribute.GenerateInMenu)
			{
				Texture2D icon = attribute.Icon ?? new Texture2D() { ResourcePath = DefaultIconPath };
				CreateSectionButton(attribute.Name, icon);
			}
		}
	}

	private void CreateSectionButton(string sectionName, Texture2D icon)
	{
		Button buttonInstance = _sectionButtonTemplate.Duplicate() as Button;
		if (buttonInstance != null)
		{
			buttonInstance.Name = sectionName;
			buttonInstance.Text = sectionName;
			buttonInstance.Icon = icon;
			_sectionButtonContainer.AddChild(buttonInstance);
		}
	}
}
