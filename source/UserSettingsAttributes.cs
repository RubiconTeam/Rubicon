namespace Rubicon;

[AttributeUsage(AttributeTargets.Class)]
public class UserSettingsSectionAttribute(string name, bool notGenerated = false) : Attribute
{
    public string Name = name;

    public bool Generated = !notGenerated;
}

/// <summary>
/// An attribute to define a slider for numeric setting fields when generating settings in the settings menu.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class UserSettingsIntStepValueAttribute(int step, int minimum = int.MinValue, int maximum = int.MaxValue) : Attribute
{
    /// <summary>
    /// Gets or sets the step value for the slider.
    /// </summary>
    /// <remarks>
    /// The step value determines the increment or decrement amount when adjusting the slider.
    /// </remarks>
    public int Step = step;

    /// <summary>
    /// Gets or sets the minimum value for the slider.
    /// </summary>
    public int Minimum = minimum;

    /// <summary>
    /// Gets or sets the maximum value for the slider.
    /// </summary>
    public int Maximum = maximum;
}

/// <summary>
/// An attribute to define a slider for numeric setting fields when generating settings in the settings menu.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class UserSettingsFloatStepValueAttribute(float step, float minimum = float.MinValue, float maximum = float.MaxValue) : Attribute
{
    /// <summary>
    /// Gets or sets the step value for the slider.
    /// </summary>
    /// <remarks>
    /// The step value determines the increment or decrement amount when adjusting the slider.
    /// </remarks>
    public float Step = step;

    /// <summary>
    /// Gets or sets the minimum value for the slider.
    /// </summary>
    public float Minimum = minimum;

    /// <summary>
    /// Gets or sets the maximum value for the slider.
    /// </summary>
    public float Maximum = maximum;
}