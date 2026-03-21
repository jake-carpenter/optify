using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Optify;

// ReSharper disable once UnusedMember.Global
public static class HostBuilderExtensions
{
    /// <summary>
    /// Register a single type options type using the conventional name or one specified on the
    /// <see cref="OptifyAttribute"/> attribute. Using this method will require reflection during runtime.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to extend.</param>
    /// <param name="options">An instance of <see cref="OptifyOptions"/> to specify customized options.</param>
    /// <typeparam name="T">The type to map configured options to.</typeparam>
    /// <returns>The extended <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder UseOptify<T>(this IHostBuilder hostBuilder, OptifyOptions options) where T : class, new()
    {
        hostBuilder.ConfigureServices((ctx, services) =>
        {
            if (options.SectionName is not { Length: > 0 } sectionName)
            {
                var type = typeof(T);
                var attribute = type
                    .GetCustomAttributes(typeof(OptifyAttribute), false)
                    .FirstOrDefault(a => a is OptifyAttribute);

                sectionName = attribute is OptifyAttribute { SectionName.Length: > 0 } attr
                    ? attr.SectionName
                    : type.Name;
            }

            services
                .AddOptions<T>()
                .Bind(ctx.Configuration.GetSection(sectionName));
        });

        return hostBuilder;
    }

    /// <summary>
    /// Register a single type options type using the conventional name or one specified on the
    /// <see cref="OptifyAttribute"/> attribute. Using this method will require reflection during runtime.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to extend.</param>
    /// <typeparam name="T">The type to map configured options to.</typeparam>
    /// <returns>The extended <see cref="IHostBuilder"/> instance.</returns>
    public static IHostBuilder UseOptify<T>(this IHostBuilder hostBuilder) where T : class, new() =>
        hostBuilder.UseOptify<T>(new OptifyOptions());
}