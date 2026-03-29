using Optify;

namespace ExampleApp;

[OptifyOptions]
public record Settings
{
    public string MagicValue { get; init; } = string.Empty;
}
