namespace Rubicon.Data.Settings.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class StepValueAttribute(float step, float min = float.MinValue, float max = float.MaxValue) : Attribute
{
	public float Step { get; set; } = step;
	public float Minimum { get; set; } = min;
	public float Maximum { get; set; } = max;
}
