using System.Collections.Immutable;
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
                    var symbol = ctx.TargetSymbol;
                    return symbol as INamedTypeSymbol;
                })
            .Where(static s => s is not null);

        context.RegisterSourceOutput(
            provider.Collect(),
            static (spc, symbols) =>
            {
                var types = symbols.OfType<INamedTypeSymbol>().Where(s => s is not null).ToImmutableArray();
                spc.AddSource(HostBuilderExtensionsSource.Filename, HostBuilderExtensionsSource.GenerateSource(types));
            });
    }
}