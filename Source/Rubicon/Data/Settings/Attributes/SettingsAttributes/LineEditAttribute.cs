namespace Rubicon.Data.Settings.Attributes
{
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
}