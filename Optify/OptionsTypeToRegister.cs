namespace Optify;

public readonly record struct OptionsTypeToRegister
{
    public readonly bool IsNull;
    public readonly string SectionName = string.Empty;
    public readonly string FullName = string.Empty;
    public readonly ValidationFlag Validation = 0;

    public OptionsTypeToRegister(string sectionName, string fullName, ValidationFlag validation)
    {
        SectionName = sectionName;
        FullName = fullName;
        Validation = validation;
    }

    public OptionsTypeToRegister()
    {
        IsNull = true;
    }
}
