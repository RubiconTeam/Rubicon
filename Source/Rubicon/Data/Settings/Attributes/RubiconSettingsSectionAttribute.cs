namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RubiconSettingsSectionAttribute : Attribute
{
	public string Name;
	public Texture2D Icon;
	public bool GenerateInMenu;

	public RubiconSettingsSectionAttribute(string name, bool generateInMenu = true, string iconPath = null)
	{
		Name = name;
		GenerateInMenu = generateInMenu;

		if (!string.IsNullOrWhiteSpace(iconPath))
			Icon = GD.Load<Texture2D>(iconPath);
	}
}
