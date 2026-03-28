namespace Optify;

/// <summary>
/// Specifies validation behaviors to apply to options registrations.
/// This enum supports bitwise combination of its member values.
/// </summary>
[Flags]
public enum ValidationFlag
{
    /// <summary>
    /// Validates options using <see cref="System.ComponentModel.DataAnnotations"/> attributes
    /// such as <see cref="System.ComponentModel.DataAnnotations.RequiredAttribute"/> and
    /// <see cref="System.ComponentModel.DataAnnotations.RangeAttribute"/>.
    /// </summary>
    DataAnnotations = 1,

    /// <summary>
    /// Validates options eagerly during host startup rather than on first access.
    /// Typically combined with <see cref="DataAnnotations"/> to catch configuration
    /// errors immediately at application start.
    /// </summary>
    OnStart = 2,
}