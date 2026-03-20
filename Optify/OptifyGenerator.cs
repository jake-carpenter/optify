using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Optify;

[Generator]
public class OptifyGenerator : IIncrementalGenerator
{
    private const string AttributeMetadataName = "Optify.OptifyAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource(
                "OptifyAttribute.g.cs",
                """
                namespace Optify;

                [AttributeUsage(AttributeTargets.Class)]
                internal sealed class OptifyAttribute : Attribute
                {
                }
                """);
        });

        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeMetadataName,
                static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                static (ctx, _) =>
                {
                    var symbol = ctx.TargetSymbol;
                    return symbol as INamedTypeSymbol;
                })
            .Where(static s => s is not null);

        context.RegisterSourceOutput(
            provider.Collect(),
            static (spc, symbols) =>
            {
                var types = symbols.OfType<INamedTypeSymbol>().Where(s => s is not null).ToImmutableArray();
                spc.AddSource("OptifyRegistration.g.cs", GenerateExtensionMethod(types));
            });
    }

    private static string GenerateExtensionMethod(ImmutableArray<INamedTypeSymbol> types)
    {
        var sb = new StringBuilder();
        sb.AppendLine("using Microsoft.Extensions.Configuration;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using Microsoft.Extensions.Options;");
        sb.AppendLine("using Microsoft.Extensions.Hosting;");
        sb.AppendLine();
        sb.AppendLine("namespace Optify;");
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
            sb.AppendLine();
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
