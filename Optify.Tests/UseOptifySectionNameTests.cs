using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Optify.Tests;

public class UseOptifySectionNameTests
{
    [Test]
    public async Task Registers_section_name_from_attribute_as_first_priority()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("NamedDummyClassSettings:X", "came-from-type-name"),
                new("OverrideNamedDummyClassSettings:X", "came-from-attribute")
            ])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-attribute");
    }

    [Test]
    public async Task Registers_section_name_from_type_name_as_second_priority()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("DummyClassSettingsA:X", "came-from-type-name")
            ])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-type-name");
    }
}