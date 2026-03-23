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