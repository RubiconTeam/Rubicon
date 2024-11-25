namespace Rubicon.Data.Settings.Attributes;

/// <summary>
/// An attribute to define a slider for numeric setting fields when generating settings in Rubicon's settings menu.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class StepValueAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the step value for the slider.
    /// </summary>
    /// <remarks>
    /// The step value determines the increment or decrement amount when adjusting the slider.
    /// </remarks>
    public float Step { get; set; }

    /// <summary>
    /// Gets or sets the minimum value for the slider.
    /// </summary>
    public float Minimum { get; set; }

    /// <summary>
    /// Gets or sets the maximum value for the slider.
    /// </summary>
    public float Maximum { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepValueAttribute"/> class.
    /// </summary>
    /// <param name="step">The step value for the slider.</param>
    /// <param name="min">The minimum value for the slider. Default is <see cref="float.MinValue"/>.</param>
    /// <param name="max">The maximum value for the slider. Default is <see cref="float.MaxValue"/>.</param>
    public StepValueAttribute(float step, float min = float.MinValue, float max = float.MaxValue)
    {
        Step = step;
        Minimum = min;
        Maximum = max;
    }
}