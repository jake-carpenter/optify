using System.ComponentModel.DataAnnotations;

namespace Optify.Tests;

[OptifyOptions]
public class DummyClassSettingsA
{
    public string? X { get; init; }
}

[OptifyOptions]
public class DummyClassSettingsB
{
    public string? X { get; init; }
}

[OptifyOptions]
public record DummyRecordSettings
{
    public string? X { get; init; }
}

[OptifyOptions(SectionName = "OverrideNamedDummyClassSettings")]
public class NamedDummyClassSettings
{
    public string? X { get; init; }
}

[OptifyOptions(SectionName = "OverrideNamedDummyRecordSettings")]
public record NamedDummyRecordSettings
{
    public string? X { get; init; }
}

public class UnmarkedDummyClassSettings
{
    public string? X { get; init; }
}

[OptifyOptions(SectionName = "")]
public class EmptySectionNameDummyClassSettings
{
    public string? X { get; init; }
}

[OptifyOptions]
public class DummySettingsWithRequiredKeyword
{
    public required string X { get; init; }
}

[OptifyOptions]
public class ValidatedDummySettings
{
    [Required]
    public string X { get; init; } = null!;
}

[OptifyOptions(Validation = ValidationFlag.DataAnnotations)]
public class AttrValidatedDummySettings
{
    [Required]
    public string? X { get; init; }
}

[OptifyOptions(Validation = ValidationFlag.DataAnnotations | ValidationFlag.OnStart)]
public class AttrValidatedOnStartDummySettings
{
    [Required]
    public string? X { get; init; }
}

[OptifyOptions(Validation = ValidationFlag.OnStart)]
public class AttrOnStartOnlyDummySettings
{
    [Required]
    public string? X { get; init; }
}
