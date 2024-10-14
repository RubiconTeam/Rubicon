namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public class StepValueAttribute(float step, float min = float.MinValue, float max = float.MaxValue) : Attribute
{
	public float Step { get; set; } = step;
	public float Minimum { get; set; } = min;
	public float Maximum { get; set; } = max;
}
