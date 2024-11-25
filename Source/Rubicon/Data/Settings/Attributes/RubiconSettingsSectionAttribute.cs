namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RubiconSettingsSectionAttribute(string name, bool generateInMenu = true, string iconPath = null, string sectionName = null): Attribute
{
	public readonly string Name = name;
	public readonly string SectionName = sectionName;
	public readonly Texture2D Icon = GD.Load<Texture2D>(iconPath);
	public readonly bool GenerateInMenu = generateInMenu;
}
