using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Optify;

public static class HostBuilderExtensionsSource
{
    public const string Filename = "OptifyRegistration.g.cs";

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
            var sectionName = type.Name;


            sb.AppendLine("            services");
            sb.AppendLine($"               .AddOptions<{fullName}>()");
            sb.AppendLine($"               .Bind(ctx.Configuration.GetSection(\"{sectionName}\"));");
        }

        sb.AppendLine("        });");
        sb.AppendLine("        return hostBuilder;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine(
            "    public static IHostBuilder UseOptify<T>(this IHostBuilder hostBuilder) where T : class, new()");
        sb.AppendLine("    {");
        sb.AppendLine("        hostBuilder.ConfigureServices((ctx, services) =>");
        sb.AppendLine("        {");
        sb.AppendLine("            services");
        sb.AppendLine("                .AddOptions<T>()");
        sb.AppendLine("                .Bind(ctx.Configuration.GetSection(typeof(T).Name));");
        sb.AppendLine("        });");

        sb.AppendLine("        return hostBuilder;");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }
}