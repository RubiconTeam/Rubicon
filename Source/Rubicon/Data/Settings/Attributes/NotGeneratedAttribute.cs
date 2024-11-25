namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class NotGeneratedAttribute : Attribute
{
	public NotGeneratedAttribute() {}
}
