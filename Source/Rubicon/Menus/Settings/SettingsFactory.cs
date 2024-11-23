namespace Rubicon.Menus.Settings;

public static class SettingsFactory
{
	public static Button CreateSectionButton(Button template, string sectionName, Texture2D icon)
	{
		if (template == null)
			throw new ArgumentNullException(nameof(template), "Template cannot be null.");

		if (template.Visible)
			GD.PushWarning("The section template cannot be visible.");
		
		Button buttonInstance = template.Duplicate() as Button;
		if (buttonInstance != null)
		{
			buttonInstance.Name = sectionName;
			buttonInstance.Text = sectionName;
			buttonInstance.Icon = icon;
			buttonInstance.Visible = true;
		}

		return buttonInstance;
	}
	
	public static VBoxContainer CreateSectionContainer(VBoxContainer template, string sectionName, bool generateInMenu)
	{
		if (template == null)
			throw new ArgumentNullException(nameof(template), "Template cannot be null.");

		if (template.Visible)
			GD.PushWarning("The section container template cannot be visible.");

		if (!generateInMenu)
			return null;

		VBoxContainer containerInstance = template.Duplicate() as VBoxContainer;
		if (containerInstance != null)
		{
			containerInstance.Name = $"{sectionName}_Container";
			containerInstance.Visible = true;
		}

		return containerInstance;
	}
}
