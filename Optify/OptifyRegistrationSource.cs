using System.Collections.Immutable;
using System.Text;

namespace Optify;

public static class OptifyRegistrationSource
{
    public const string Filename = "OptifyRegistration.g.cs";

    public static string GenerateSource(ImmutableArray<OptionsTypeToRegister> optionsToRegister)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            """
            using Microsoft.Extensions.Configuration;
            using Microsoft.Extensions.DependencyInjection;
            using Microsoft.Extensions.Options;
            using Microsoft.Extensions.Hosting;

            namespace Optify;

            public static partial class OptifyRegistration
            {
                /// <summary>
                /// Register all configurable options types in the current assembly marked with the
                /// <see cref="OptifyOptionsAttribute"/> attribute.
                /// </summary>
                /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to extend.</param>
                /// <returns>The extended <see cref="IHostBuilder"/> instance.</returns>
                public static IHostBuilder UseOptify(this IHostBuilder hostBuilder)
                {
                    hostBuilder.ConfigureServices((ctx, services) =>
                    {
            """);
        sb.AppendLine(GenerateRegistrationStatements(optionsToRegister));
        sb.AppendLine(
            """
                    });
                
                    return hostBuilder;
                }
            }
            """);

        return sb.ToString();
    }

    private static string GenerateRegistrationStatements(ImmutableArray<OptionsTypeToRegister> optionsToRegister)
    {
        var sb = new StringBuilder();

        foreach (var optionsType in optionsToRegister)
        {
            sb.AppendLine("            services");
            sb.AppendLine($"               .AddOptions<{optionsType.FullName}>()");
            sb.AppendLine($"               .Bind(ctx.Configuration.GetSection(\"{optionsType.SectionName}\"));");
        }

        return sb.ToString();
    }
}