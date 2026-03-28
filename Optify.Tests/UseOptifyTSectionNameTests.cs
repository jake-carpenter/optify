using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Optify.Tests;

public class UseOptifyTSectionNameTests
{
    [Test]
    public async Task Registers_section_name_from_extension_config_as_first_priority()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("NamedDummyClassSettings:X", "came-from-type-name"), // wrong
                new("OverrideNamedDummyClassSettings:X", "came-from-attribute"), // wrong
                new("UseThisOne:X", "came-from-config-passed-to-extension")
            ])
            .UseOptify<NamedDummyClassSettings>(new OptifyConfiguration { SectionName = "UseThisOne" })
            .Build();
        
        var options = host.Services.GetRequiredService<IOptions<NamedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-config-passed-to-extension");
    }
    
    [Test]
    public async Task Registers_section_name_from_attribute_as_second_priority()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("NamedDummyClassSettings:X", "came-from-type-name"), // wrong
                new("OverrideNamedDummyClassSettings:X", "came-from-attribute"),
            ])
            .UseOptify<NamedDummyClassSettings>(new OptifyConfiguration()) // no SectionName
            .Build();
        
        var options = host.Services.GetRequiredService<IOptions<NamedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-attribute");
    }

    [Test]
    public async Task Registers_section_name_from_type_name_as_third_priority()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("DummyClassSettingsA:X", "came-from-type-name")
            ])
            .UseOptify<DummyClassSettingsA>(new OptifyConfiguration()) // no SectionName, type has no attribute SectionName
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-type-name");
    }
}