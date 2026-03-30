using System.Collections.Immutable;
using Scriban;
using Scriban.Runtime;

namespace Optify;

public static class OptifyRegistrationSource
{
    public const string Filename = "OptifyRegistration.g.cs";

    private static readonly Template SourceTemplate = Template.Parse(TemplateString);

    public static string GenerateSource(ImmutableArray<OptionsTypeToRegister> optionsToRegister)
    {
        var options = new ScriptArray();

        foreach (var opt in optionsToRegister)
        {
            options.Add(
                new ScriptObject
                {
                    ["full_name"] = opt.FullName,
                    ["section_name"] = opt.SectionName,
                    ["has_data_annotations"] = opt.Validation.HasFlag(ValidationFlag.DataAnnotations),
                    ["has_validate_on_start"] = opt.Validation.HasFlag(ValidationFlag.OnStart),
                }
            );
        }

        var model = new ScriptObject { ["options"] = options };
        var context = new TemplateContext();
        context.PushGlobal(model);

        return SourceTemplate.Render(context);
    }

    private const string TemplateString = """
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
        {{~ for opt in options ~}}
        {{~ if !for.first }}{{ "\n" }}{{ end ~}}
                    services
                        .AddOptions<{{ opt.full_name }}>()
                        .Bind(ctx.Configuration.GetSection("{{ opt.section_name }}"))
        {{- if opt.has_data_annotations }}
                        .ValidateDataAnnotations()
        {{- end }}
        {{- if opt.has_validate_on_start }}
                        .ValidateOnStart()
        {{- end }};
        {{~ end ~}}
                });

                return hostBuilder;
            }

            /// <summary>
            /// Register all configurable options types in the current assembly marked with the
            /// <see cref="OptifyOptionsAttribute"/> attribute.
            /// </summary>
            /// <param name="hostBuilder">The <see cref="IHostBuilder"/> instance to extend.</param>
            /// <param name="validation">Specifies which validation behaviors to apply to the options registration.</param>
            /// <returns>The extended <see cref="IHostBuilder"/> instance.</returns>
            public static IHostBuilder UseOptify(this IHostBuilder hostBuilder, ValidationFlag validation)
            {
                hostBuilder.ConfigureServices((ctx, services) =>
                {
        {{~ for opt in options ~}}
        {{~ if !for.first }}{{ "\n" }}{{ end ~}}
                    services
                        .AddOptions<{{ opt.full_name }}>()
                        .Bind(ctx.Configuration.GetSection("{{ opt.section_name }}"))
                        .MaybeAddValidation<{{ opt.full_name }}>(validation)
        {{- if opt.has_data_annotations }}
                        .ValidateDataAnnotations()
        {{- end }}
        {{- if opt.has_validate_on_start }}
                        .ValidateOnStart()
        {{- end }};
        {{~ end ~}}
                });

                return hostBuilder;
            }
        }
        """;
}
