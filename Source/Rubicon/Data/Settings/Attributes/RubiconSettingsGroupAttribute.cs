namespace Rubicon.Data.Settings.Attributes;

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