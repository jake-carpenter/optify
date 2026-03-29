using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Optify.Tests;

public class UseOptifyTests
{
    [Test]
    public async Task Registers_all_marked_classes()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                .. TestData.AllTestSettings,
                new("DummyClassSettingsA:X", "a-value"),
                new("DummyClassSettingsB:X", "b-value"),
            ])
            .UseOptify()
            .Build();

        var optionsA = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();
        var optionsB = host.Services.GetRequiredService<IOptions<DummyClassSettingsB>>();

        using var _ = Assert.Multiple();
        await Assert.That(optionsA.Value.X).IsEqualTo("a-value");
        await Assert.That(optionsB.Value.X).IsEqualTo("b-value");
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_all_marked_records(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([.. TestData.AllTestSettings, new("DummyRecordSettings:X", value)])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_marked_class_with_provided_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([.. TestData.AllTestSettings, new("OverrideNamedDummyClassSettings:X", value)])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_marked_record_with_provided_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([.. TestData.AllTestSettings, new("OverrideNamedDummyRecordSettings:X", value)])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    public async Task Should_not_register_unmarked_types()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([.. TestData.AllTestSettings, new("UnmarkedDummyClassSettings:Setting", "arbitrary")])
            .UseOptify()
            .Build();

        var options = host.Services.GetService<IOptions<UnmarkedDummyClassSettings>>();

        await Assert.That(options!.Value.X).IsNull();
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Should_allow_required_properties_on_type(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([.. TestData.AllTestSettings, new("DummySettingsWithRequiredKeyword:X", value)])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummySettingsWithRequiredKeyword>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }
}
