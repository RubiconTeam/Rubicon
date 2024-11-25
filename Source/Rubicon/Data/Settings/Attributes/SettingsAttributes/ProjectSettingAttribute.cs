namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ProjectSettingAttribute(string name) : Attribute
{
	public string SettingName { get; set; } = name;
}