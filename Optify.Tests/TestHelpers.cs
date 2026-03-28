using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Hosting;

namespace Optify.Tests;

static class TestHelpers
{
    extension(IHostBuilder hostBuilder)
    {
        internal IHostBuilder IncludeConfiguration(params KeyValuePair<string, string?>[] configuration)
        {
            return hostBuilder.ConfigureAppConfiguration(config =>
            {
                var distinctPairsTakingTheLatest = new Dictionary<string, string?>();

                foreach (var (key, value) in configuration)
                {
                    if (value is null)
                    {
                        distinctPairsTakingTheLatest.Remove(key);
                    }
                    else
                    {
                        distinctPairsTakingTheLatest[key] = value;
                    }
                }

                config.AddInMemoryCollection(distinctPairsTakingTheLatest);
            });
        }
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
