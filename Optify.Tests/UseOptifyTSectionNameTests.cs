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
                .. TestData.AllTestSettings,
                new("NamedDummyClassSettings:X", "came-from-type-name"), // wrong
                new("OverrideNamedDummyClassSettings:X", "came-from-attribute"), // wrong
                new("UseThisOne:X", "came-from-config-passed-to-extension"),
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
                .. TestData.AllTestSettings,
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
            .IncludeConfiguration([.. TestData.AllTestSettings, new("DummyClassSettingsA:X", "came-from-type-name")])
            .UseOptify<DummyClassSettingsA>(new OptifyConfiguration()) // no SectionName, type has no attribute SectionName
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-type-name");
    }

    [Test]
    public async Task Registers_section_name_from_type_name_when_no_attribute_present()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                .. TestData.AllTestSettings,
                new("UnmarkedDummyClassSettings:X", "came-from-type-name"),
            ])
            .UseOptify<UnmarkedDummyClassSettings>(new OptifyConfiguration())
            .Build();

        var options = host.Services.GetRequiredService<IOptions<UnmarkedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-type-name");
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.NullOrWhitespaceTestData))]
    public async Task Registers_section_name_from_type_name_when_configuration_section_name_is_null_or_whitespace(
        string? sectionName
    )
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                .. TestData.AllTestSettings,
                new("UnmarkedDummyClassSettings:X", "came-from-type-name"),
            ])
            .UseOptify<UnmarkedDummyClassSettings>(new OptifyConfiguration { SectionName = sectionName })
            .Build();

        var options = host.Services.GetRequiredService<IOptions<UnmarkedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-type-name");
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.NullOrWhitespaceTestData))]
    public async Task Registers_section_name_from_type_name_when_both_configuration_and_attribute_section_names_are_null_or_whitespace(
        string? sectionName
    )
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                .. TestData.AllTestSettings,
                new("EmptySectionNameDummyClassSettings:X", "came-from-type-name"),
            ])
            .UseOptify<EmptySectionNameDummyClassSettings>(new OptifyConfiguration { SectionName = sectionName })
            .Build();

        var options = host.Services.GetRequiredService<IOptions<EmptySectionNameDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-type-name");
    }

    [Test]
    public async Task Registers_section_name_from_type_name_when_configuration_is_null_and_no_attribute()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                .. TestData.AllTestSettings,
                new("UnmarkedDummyClassSettings:X", "came-from-type-name"),
            ])
            .UseOptify<UnmarkedDummyClassSettings>(null!)
            .Build();

        var options = host.Services.GetRequiredService<IOptions<UnmarkedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("came-from-type-name");
    }
}
