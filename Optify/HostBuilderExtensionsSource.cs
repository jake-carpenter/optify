using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Optify;

public static class HostBuilderExtensionsSource
{
    public const string Filename = "OptifyRegistration.g.cs";

    private static string UseOptifyGenericExtensionSource =
        """
            public static IHostBuilder UseOptify<T>(this IHostBuilder hostBuilder) where T : class, new()
            {
                hostBuilder.ConfigureServices((ctx, services) =>
                {
                    var type = typeof(T);
                    var attribute = type
                        .GetCustomAttributes(typeof(OptifyAttribute), false)
                        .FirstOrDefault(attr => attr is OptifyAttribute);

                    var sectionName = attribute is OptifyAttribute {SectionName.Length: > 0} attr
                        ? attr.SectionName
                        : type.Name;

                    services
                        .AddOptions<T>()
                        .Bind(ctx.Configuration.GetSection(sectionName));
                });
                return hostBuilder;
            }
        """;

    public static string GenerateSource(ImmutableArray<INamedTypeSymbol> types)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            """
            using Microsoft.Extensions.Configuration;
            using Microsoft.Extensions.DependencyInjection;
            using Microsoft.Extensions.Options;
            using Microsoft.Extensions.Hosting;

            namespace Optify;
            """);

        sb.AppendLine();
        sb.AppendLine("public static partial class OptifyRegistration");
        sb.AppendLine("{");
        sb.AppendLine(
            "    public static IHostBuilder UseOptify(this IHostBuilder hostBuilder)");
        sb.AppendLine("    {");
        sb.AppendLine("        hostBuilder.ConfigureServices((ctx, services) =>");
        sb.AppendLine("        {");

        foreach (var type in types)
        {
            var fullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "");
            var attribute = type.GetAttributes()
                .FirstOrDefault(x => x?.AttributeClass?.Name == OptifyAttributeSource.ClassName);
            var sectionNameArg = attribute?.NamedArguments
                .FirstOrDefault(x => x.Key == OptifyAttributeSource.SectionNamePropertyName);
            var sectionName = sectionNameArg?.Value.Value as string ?? type.Name;
            sb.AppendLine("            services");
            sb.AppendLine($"               .AddOptions<{fullName}>()");
            sb.AppendLine($"               .Bind(ctx.Configuration.GetSection(\"{sectionName}\"));");
        }

        sb.AppendLine("        });");
        sb.AppendLine("        return hostBuilder;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine(UseOptifyGenericExtensionSource);
        sb.AppendLine("}");

        return sb.ToString();
    }
}