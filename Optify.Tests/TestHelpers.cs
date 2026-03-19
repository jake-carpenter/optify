using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Optify.Tests;

static class TestHelpers
{
    extension(IHostBuilder hostBuilder)
    {
        internal IHostBuilder IncludeConfiguration(params KeyValuePair<string, string?>[] configuration)
        {
            return hostBuilder.ConfigureAppConfiguration(config => config.AddInMemoryCollection(configuration));
        }
    }
}
