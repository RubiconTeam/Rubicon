namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class LineEditAttribute : Attribute
{
	public string PlaceholderText { get; set; }
	public LineEditAttribute(string name)
	{
		PlaceholderText = name;
	}
}