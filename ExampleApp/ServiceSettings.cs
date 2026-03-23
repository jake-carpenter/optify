using Optify;

namespace ExampleApp;

[OptifyOptions(SectionName = "ExternalServiceSettings")]
public record ServiceSettings
{
    public string BaseUrl { get; init; } = string.Empty;
}