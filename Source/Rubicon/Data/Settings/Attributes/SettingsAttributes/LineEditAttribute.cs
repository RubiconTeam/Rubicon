namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class LineEditAttribute(string name) : Attribute
{
	public string PlaceholderText { get; set; } = name;
}