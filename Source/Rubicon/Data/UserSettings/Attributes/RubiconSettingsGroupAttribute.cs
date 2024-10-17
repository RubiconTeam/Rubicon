namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class RubiconSettingsGroupAttribute : Attribute
{
	public string SectionName { get; set; }
	public RubiconSettingsGroupAttribute(string sectionName)
	{
		SectionName = sectionName;
	}
}
