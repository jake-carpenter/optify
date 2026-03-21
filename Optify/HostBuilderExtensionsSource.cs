using System.Collections.Immutable;
using System.Text;

namespace Optify;

public static class HostBuilderExtensionsSource
{
    public const string Filename = "OptifyRegistration.g.cs";

    private const string UseOptifyGenericExtensionSource =
        """
            /// <summary>
            /// Register a single type options type using the conventional name or one specified on the
            /// <see cref="OptifyAttribute"/> attribute. Using this method will require reflection during runtime.
            /// </summary>
            /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to extend.</param>
            /// <typeparam name="T">The type to map configured options to.</typeparam>
            /// <returns>The extended <see cref="IHostBuilder"/> instance.</returns>
            public static IHostBuilder UseOptify<T>(this IHostBuilder hostBuilder) where T : class, new()
            {
                hostBuilder.ConfigureServices((ctx, services) =>
                {
                    var type = typeof(T);
                    var attribute = type
                        .GetCustomAttributes(typeof(OptifyAttribute), false)
                        .FirstOrDefault(attr => attr is OptifyAttribute);
            
                    var sectionName = attribute is OptifyAttribute { SectionName.Length: > 0 } attr
                        ? attr.SectionName
                        : type.Name;
            
                    services
                        .AddOptions<T>()
                        .Bind(ctx.Configuration.GetSection(sectionName));
                });
                return hostBuilder;
            }
        """;

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
                /// <see cref="OptifyAttribute"/> attribute.
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
            """);
        sb.AppendLine();
        sb.AppendLine(UseOptifyGenericExtensionSource);
        sb.AppendLine("}");

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