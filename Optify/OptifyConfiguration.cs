namespace Optify;

/// <summary>
/// Configuration options for customizing how Optify registers and validates options types.
/// </summary>
public record OptifyConfiguration
{
    /// <summary>
    /// The configuration section name to bind to. When <c>null</c> or empty, the section name
    /// is resolved from the <see cref="OptifyOptionsAttribute"/> on the options type, or falls
    /// back to the type name.
    /// </summary>
    public string? SectionName { get; set; }

    /// <summary>
    /// Specifies which validation behaviors to apply to the options registration.
    /// Supports bitwise combination of <see cref="ValidationFlag"/> values.
    /// </summary>
    public ValidationFlag Validation { get; set; }
}
