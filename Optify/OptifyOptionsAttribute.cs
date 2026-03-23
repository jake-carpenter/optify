using Microsoft.Extensions.Configuration;

namespace Optify;

/// <summary>
/// Attribute to mark a type that will be bound to a <see cref="IConfigurationSection"/>
/// using the <see cref="IConfiguration"/> API.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class OptifyOptionsAttribute : Attribute
{
    /// <summary>
    /// Manually specified <see cref="IConfigurationSection"/> name to bind to.
    /// If not specified, the type name will be used as a convention.
    /// </summary>
    public string? SectionName { get; set; }
}