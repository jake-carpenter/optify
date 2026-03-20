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
            .IncludeConfiguration([new("DummyClassSettingsA:X", value)])
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
            .IncludeConfiguration([new("DummyRecordSettings:X", value)])
            .UseOptify<DummyRecordSettings>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    public async Task Should_not_register_other_types()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("DummyClassSettingsA:X", "one"), new("DummyClassSettingsB:X", "two")])
            .UseOptify<DummyClassSettingsB>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsNull();
    }
}