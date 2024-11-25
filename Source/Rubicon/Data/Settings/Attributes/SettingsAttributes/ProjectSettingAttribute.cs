namespace Rubicon.Data.Settings.Attributes;

/// <summary>
/// An attribute to automatically populate a field with a value from a Godot project setting.
/// </summary>
/// <remarks>
/// This attribute binds a field to a specific Godot project setting path, provided via the <paramref name="name"/> parameter.
/// Note: Enum fields will be serialized as ints due to constraints with ConfigFile (it sucks).
/// </remarks>
[AttributeUsage(AttributeTargets.Field)]
public class ProjectSettingAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the path to the Godot project setting.
    /// </summary>
    public string SettingName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectSettingAttribute"/> class.
    /// </summary>
    /// <param name="name">The path to the Godot project setting to bind to the field.</param>
    public ProjectSettingAttribute(string name)
    {
        SettingName = name;
    }
}