namespace Optify;

public readonly record struct OptionsTypeToRegister
{
    public readonly bool IsNull;
    public readonly string SectionName = string.Empty;
    public readonly string FullName = string.Empty;

    public OptionsTypeToRegister(string sectionName, string fullName)
    {
        SectionName = sectionName;
        FullName = fullName;
    }

    public OptionsTypeToRegister()
    {
        IsNull = true;
    }
}
