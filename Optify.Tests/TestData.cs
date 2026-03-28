namespace Optify.Tests;

public class TestData
{
    /// <summary>
    /// A reusable test data source just to avoid hard-coded arbitrary strings.
    /// </summary>
    public static IEnumerable<string> GenericStringTestData()
    {
        yield return "one";
        yield return "two";
    }

    /// <summary>
    /// A valid entry for all properties on all dummy settings types in the testing project.
    /// Tests should start with this and override what is needed within the test.
    /// </summary>
    internal static KeyValuePair<string, string?>[] AllTestSettings =
    [
        new($"{nameof(DummyClassSettingsA)}:X", "valid"),
        new($"{nameof(DummyClassSettingsB)}:X", "valid"),
        new($"{nameof(DummyRecordSettings)}:X", "valid"),
        new("OverrideNamedDummyClassSettings:X", "valid"),
        new("OverrideNamedDummyRecordSettings:X", "valid"),
        new($"{nameof(UnmarkedDummyClassSettings)}:X", "valid"),
        new($"{nameof(DummySettingsWithRequiredKeyword)}:X", "valid"),
        new($"{nameof(ValidatedDummySettings)}:X", "valid"),
        new($"{nameof(AttrValidatedDummySettings)}:X", "valid"),
        new($"{nameof(AttrValidatedOnStartDummySettings)}:X", "valid"),
    ];
}