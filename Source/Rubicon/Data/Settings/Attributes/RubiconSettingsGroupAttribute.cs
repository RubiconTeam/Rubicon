namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false)]
public class RubiconSettingsGroupAttribute(string sectionName) : Attribute
{
	public string SectionName { get; set; } = sectionName;
}
