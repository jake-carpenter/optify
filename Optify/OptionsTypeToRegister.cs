using Microsoft.CodeAnalysis;

namespace Optify;

public readonly record struct OptionsTypeToRegister
{
    public bool IsValid { get; }
    public string SectionName { get; } = string.Empty;
    public string FullName { get; } = string.Empty;
    public ValidationFlag Validation { get; }

    public OptionsTypeToRegister(AttributeData attributeData, INamedTypeSymbol targetType)
    {
        FullName = targetType.ToDisplayString();
        SectionName = targetType.Name;
        IsValid = true;

        foreach (var argument in attributeData.NamedArguments)
        {
            switch (argument.Key)
            {
                case nameof(OptifyOptionsAttribute.SectionName):
                    SectionName = argument.Value.Value as string ?? targetType.Name;
                    break;
                case nameof(OptifyOptionsAttribute.Validation):
                    Validation = argument.Value.Value is int intValue ? (ValidationFlag)intValue : 0;
                    break;
            }
        }
    }

    public OptionsTypeToRegister()
    {
        IsValid = false;
    }
}
