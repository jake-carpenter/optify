using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Optify.Tests;

public class OptifyTests
{
    [Test]
    [Arguments("foo")]
    [Arguments("bar")]
    public async Task Optify_registers_requested_generic_type_with_name_of_class_for_section(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("DummySettingsA:A", value)])
            .UseOptify<DummySettingsA>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummySettingsA>>();

        await Assert.That(options.Value.A).IsEqualTo(value);
    }

    [Test]
    public async Task Optify_does_not_register_other_types_when_generic_type_is_specified()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("DummySettingsA:A", "one"), new("DummySettingsB:B", "two")])
            .UseOptify<DummySettingsB>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummySettingsA>>();

        await Assert.That(options.Value.A).IsNull();
    }

    [Test]
    public async Task Optify_registers_all_marked_types_when_no_generic_type_is_specified()
    {
        var host = new HostBuilder()
            .IncludeConfiguration(new("DummySettingsA:A", "one"), new("DummySettingsB:B", "two"))
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummySettingsA>>();
        var options2 = host.Services.GetRequiredService<IOptions<DummySettingsB>>();

        using var _ = Assert.Multiple();
        await Assert.That(options.Value.A).IsEqualTo("one");
        await Assert.That(options2.Value.B).IsEqualTo("two");
    }

    [Test]
    public async Task Optify_does_not_register_unmarked_types_when_no_generic_type_is_specified()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                new("DummySettingsA:A", "one"),
                new("DummySettingsB:B", "two"),
                new("UnmarkedSettings:Setting", "three")
            ])
            // .UseOptify()
            .Build();

        var options = host.Services.GetService<IOptions<UnmarkedSettings>>();

        await Assert.That(options!.Value.Setting).IsNull();
    }
}