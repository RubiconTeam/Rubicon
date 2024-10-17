namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public class NotGeneratedAttribute : Attribute
{
	public NotGeneratedAttribute() {}
}
