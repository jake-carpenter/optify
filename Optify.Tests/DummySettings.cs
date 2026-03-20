namespace Optify.Tests;

[Optify]
public class DummyClassSettingsA
{
    public string? X { get; init; }
}

[Optify]
public class DummyClassSettingsB
{
    public string? X { get; init; }
}

[Optify]
public record DummyRecordSettings
{
    public string? X { get; init; }
}

[Optify(SectionName = "OverrideNamedDummyClassSettings")]
public class NamedDummyClassSettings
{
    public string? X { get; init; }
}

[Optify(SectionName = "OverrideNamedDummyRecordSettings")]
public record NamedDummyRecordSettings
{
    public string? X { get; init; }
}

public class UnmarkedDummyClassSettings
{
    public string? X { get; init; }
}