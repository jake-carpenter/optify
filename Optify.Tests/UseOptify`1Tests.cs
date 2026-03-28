using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Optify.Tests;

public class UseOptifyTTests
{
    [Test]
    [Arguments("foo")]
    [Arguments("bar")]
    public async Task Registers_class_using_convention_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("DummyClassSettingsA:X", value)
            ])
            .UseOptify<DummyClassSettingsA>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [Arguments("foo")]
    [Arguments("bar")]
    public async Task Registers_record_using_convention_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("DummyRecordSettings:X", value)
            ])
            .UseOptify<DummyRecordSettings>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    public async Task Registers_class_using_specified_name_on_attribute()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("OverrideNamedDummyClassSettings:X", "one")
            ])
            .UseOptify<NamedDummyClassSettings>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    public async Task Registers_record_using_specified_name_on_attribute()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("OverrideNamedDummyRecordSettings:X", "one")
            ])
            .UseOptify<NamedDummyRecordSettings>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    public async Task Registers_class_using_specified_name_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("OverrideDummyClassSettingsA:X", "one")
            ])
            .UseOptify<DummyClassSettingsA>(new OptifyConfiguration { SectionName = "OverrideDummyClassSettingsA" })
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    public async Task Registers_record_using_specified_name_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("OverrideDummyRecordSettings:X", "one")
            ])
            .UseOptify<DummyRecordSettings>(new OptifyConfiguration { SectionName = "OverrideDummyRecordSettings" })
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    public async Task Should_not_register_other_types()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("DummyClassSettingsA:X", "one"), new("DummyClassSettingsB:X", "two")
            ])
            .UseOptify<DummyClassSettingsB>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsNull();
    }

    [Test]
    public async Task Should_allow_required_properties_on_type()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("DummySettingsWithRequiredKeyword:X", "one")
            ])
            // This would be a type error if this test is going to fail.
            .UseOptify<DummySettingsWithRequiredKeyword>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummySettingsWithRequiredKeyword>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }
}