namespace Rubicon.Data.Settings.Attributes;

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