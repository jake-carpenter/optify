using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Optify.Tests;

public static class TestHelpers
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
}