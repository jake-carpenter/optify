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

                // At this point, ctx.Attributes *only* contains OptifyOptionsAttribute, even
                // when other attributes are on the type. A user should receive a compiler error
                // CS0579 if they added more than one [OptifyOptions]. However, generated source
                // may be able to bypass that. In such a case, we're just going to assume
                // that we'll take the first one.
                return new OptionsTypeToRegister(ctx.Attributes[0], target);
            }
        );

        context.RegisterSourceOutput(
            provider.Where(static opt => opt.IsValid).Collect(),
            static (spc, types) =>
            {
                spc.AddSource(OptifyRegistrationSource.Filename, OptifyRegistrationSource.GenerateSource(types));
            }
        );
    }
}
