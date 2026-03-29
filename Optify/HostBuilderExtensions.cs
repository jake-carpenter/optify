using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Optify;

// ReSharper disable once UnusedMember.Global
public static class HostBuilderExtensions
{
    /// <summary>
    /// Register a single type options type using the conventional name or one specified on the
    /// <see cref="OptifyOptionsAttribute"/> attribute. Using this method will require reflection during runtime.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to extend.</param>
    /// <param name="configuration">An instance of <see cref="OptifyConfiguration"/> to specify customized options.</param>
    /// <typeparam name="T">The type to map configured options to.</typeparam>
    /// <returns>The extended <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder UseOptify<T>(this IHostBuilder hostBuilder, OptifyConfiguration configuration)
        where T : class
    {
        hostBuilder.ConfigureServices(
            (ctx, services) =>
            {
                var type = typeof(T);
                var attribute = type.GetCustomAttributes(typeof(OptifyOptionsAttribute), false)
                    .FirstOrDefault(a => a is OptifyOptionsAttribute);

                // Can come from 3 sources. Order of precedence:
                // 1. `configuration.SectionName`
                // 2. Attribute `SectionName`
                // 3. `typeof(T).Name`
                var sectionName = configuration.SectionName;
                var validation = configuration.Validation;

                if (attribute is OptifyOptionsAttribute attr)
                {
                    sectionName ??= string.IsNullOrWhiteSpace(attr.SectionName) ? type.Name : attr.SectionName;
                    validation |= attr.Validation;
                }

                services
                    .AddOptions<T>()
                    .Bind(ctx.Configuration.GetSection(sectionName!))
                    .MaybeAddValidation(validation);
            }
        );

        return hostBuilder;
    }

    /// <summary>
    /// Register a single type options type using the conventional name or one specified on the
    /// <see cref="OptifyOptionsAttribute"/> attribute. Using this method will require reflection during runtime.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to extend.</param>
    /// <typeparam name="T">The type to map configured options to.</typeparam>
    /// <returns>The extended <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder UseOptify<T>(this IHostBuilder hostBuilder)
        where T : class => hostBuilder.UseOptify<T>(new OptifyConfiguration());
}
