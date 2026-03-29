using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Optify;

[Generator]
public class OptifyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            "Optify.OptifyOptionsAttribute",
            static (node, _) => node is ClassDeclarationSyntax or RecordDeclarationSyntax,
            static (ctx, _) =>
            {
                // Register a null object value that we'll filter for later.
                if (ctx.TargetSymbol is not INamedTypeSymbol target)
                    return new OptionsTypeToRegister();

                var fullName = target.ToDisplayString(
                    SymbolDisplayFormat.FullyQualifiedFormat.WithGlobalNamespaceStyle(
                        SymbolDisplayGlobalNamespaceStyle.Omitted
                    )
                );
                var sectionNameArg = ctx.Attributes[0]
                    .NamedArguments.FirstOrDefault(static x => x.Key == nameof(OptifyOptionsAttribute.SectionName));
                var sectionName = sectionNameArg.Value.Value as string ?? ctx.TargetSymbol.Name;
                var validationArg = ctx.Attributes[0]
                    .NamedArguments.FirstOrDefault(static x => x.Key == nameof(OptifyOptionsAttribute.Validation));
                var validation = validationArg.Value.Value is int v ? v : 0;

                return new OptionsTypeToRegister(sectionName, fullName, (ValidationFlag)validation);
            }
        );

        context.RegisterSourceOutput(
            provider.Where(static opt => !opt.IsNull).Collect(),
            static (spc, types) =>
            {
                spc.AddSource(OptifyRegistrationSource.Filename, OptifyRegistrationSource.GenerateSource(types));
            }
        );
    }
}
