namespace Rubicon.Data.Settings.Attributes;

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