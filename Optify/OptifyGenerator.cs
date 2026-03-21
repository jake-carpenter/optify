using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Optify;

[Generator]
public class OptifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource(OptifyAttributeSource.Filename, OptifyAttributeSource.Source);
        });

        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                OptifyAttributeSource.AttributeMetadataName,
                static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
                static (ctx, _) =>
                {
                    // Register a null object value that we'll filter for later.
                    if (ctx.TargetSymbol is not INamedTypeSymbol target)
                        return new OptionsTypeToRegister();

                    var fullName = target.ToDisplayString(
                        SymbolDisplayFormat.FullyQualifiedFormat
                            .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted));
                    var sectionNameArg = ctx.Attributes[0].NamedArguments
                        .FirstOrDefault(x => x.Key == OptifyAttributeSource.SectionNamePropertyName);
                    var sectionName = sectionNameArg.Value.Value as string ?? ctx.TargetSymbol.Name;

                    return new OptionsTypeToRegister(sectionName, fullName);
                });

        context.RegisterSourceOutput(
            provider.Where(static opt => !opt.IsNull).Collect(),
            static (spc, types) =>
            {
                spc.AddSource(
                    HostBuilderExtensionsSource.Filename,
                    HostBuilderExtensionsSource.GenerateSource(types));
            });
    }
}