namespace Rubicon;

/// <summary>
/// An attribute to define a string field as a line edit setting in Rubicon's settings menu.
/// </summary>
/// <remarks>
/// This attribute customizes a line edit box for string input in the settings menu. 
/// The <paramref name="name"/> parameter specifies the placeholder text displayed in the line edit box 
/// when the field is empty.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public class LineEditAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the placeholder text for the line edit box.
    /// </summary>
    /// <remarks>
    /// Placeholder text provides a hint or description of the expected input.
    /// </remarks>
    public string PlaceholderText { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LineEditAttribute"/> class.
    /// </summary>
    /// <param name="name">The placeholder text to display in the line edit box.</param>
    public LineEditAttribute(string name)
    {
        PlaceholderText = name;
    }
}

/// <summary>
/// An attribute to specify that a field should not be automatically generated in Rubicon's settings menu.
/// </summary>
/// <remarks>
/// Fields marked with this attribute will be excluded from automatic generation in the settings menu,
/// but will still be accessible as part of the class's data. This is particularly useful for fields 
/// that are intended to store non-configurable information, such as temporal save data.
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public class NotGeneratedAttribute : Attribute
{   
    /// <summary>
    /// Initializes a new instance of the <see cref="NotGeneratedAttribute"/> class.
    /// </summary>
    public NotGeneratedAttribute() {}
}

/// <summary>
/// An attribute to define a group or subsection in Rubicon's settings menu by creating a header / separator label.
/// </summary>
/// <remarks>
/// This attribute can be applied to classes or fields to specify a grouping in the settings menu. 
/// It creates a header label node that separates settings in between. 
/// 
/// Note: This attribute cannot be used alongside <see cref="NotGeneratedAttribute"/>, 
/// as the setting must be generated in the settings menu for the header to be created.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false)]
public class RubiconSettingsGroupAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name of the section to display as the header label in the settings menu.
    /// </summary>
    public string SectionName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RubiconSettingsGroupAttribute"/> class with the specified section name.
    /// </summary>
    /// <param name="sectionName">The name of the section to display as the header.</param>
    public RubiconSettingsGroupAttribute(string sectionName)
    {
        SectionName = sectionName;
    }
}

/// <summary>
/// An attribute to define a section in Rubicon's settings menu, displayed as a button on the left-side panel.
/// </summary>
/// <remarks>
/// This attribute allows creating a new section in the settings menu. The section is represented by a button on the 
/// left panel, and it can contain a group of settings. Additional options allow customization of the button's name, 
/// icon, and whether the section is automatically generated in the menu or only serialized in the settings file.
/// If <paramref name="SectionName"/> is specified, a Rubicon settings group separator is added at the top of the list.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RubiconSettingsSectionAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the section, used as the button label in the settings menu.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Gets the optional name of the section group separator displayed at the top of the section.
    /// </summary>
    public readonly string SectionName;

    /// <summary>
    /// Gets the icon for the section button in the settings menu.
    /// </summary>
    public readonly Texture2D Icon;

    /// <summary>
    /// Gets a value indicating whether the section and its contents should be automatically generated in the settings menu.
    /// </summary>
    public readonly bool GenerateInMenu;

    /// <summary>
    /// Initializes a new instance of the <see cref="RubiconSettingsSectionAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the section, used as the button label in the settings menu.</param>
    /// <param name="generateInMenu">
    /// Indicates whether the section and its contents should be generated in the settings menu.
    /// If <c>false</c>, the section is serialized in the settings file but not displayed in the menu.
    /// </param>
    /// <param name="iconPath">The optional path to an icon for the section button.</param>
    /// <param name="sectionName">
    /// The optional name of a Rubicon settings group separator to display at the top of the section.
    /// </param>
    public RubiconSettingsSectionAttribute(string name, bool generateInMenu = true, string iconPath = null, string sectionName = null)
    {
        Name = name;
        GenerateInMenu = generateInMenu;
        SectionName = sectionName;
        Icon = iconPath != null ? GD.Load<Texture2D>(iconPath) : null;
    }
}

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