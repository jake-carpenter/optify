using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Optify.Tests;

public class UseOptifyTTests
{
    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_class_using_convention_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("DummyClassSettingsA:X", value)
            ])
            .UseOptify<DummyClassSettingsA>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_record_using_convention_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("DummyRecordSettings:X", value)
            ])
            .UseOptify<DummyRecordSettings>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_class_using_specified_name_on_attribute(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("OverrideNamedDummyClassSettings:X", value)
            ])
            .UseOptify<NamedDummyClassSettings>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_record_using_specified_name_on_attribute(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("OverrideNamedDummyRecordSettings:X", value)
            ])
            .UseOptify<NamedDummyRecordSettings>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_class_using_specified_name_from_extension(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("OverrideDummyClassSettingsA:X", value)
            ])
            .UseOptify<DummyClassSettingsA>(new OptifyConfiguration { SectionName = "OverrideDummyClassSettingsA" })
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Registers_record_using_specified_name_from_extension(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("OverrideDummyRecordSettings:X", value)
            ])
            .UseOptify<DummyRecordSettings>(new OptifyConfiguration { SectionName = "OverrideDummyRecordSettings" })
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    public async Task Should_not_register_other_types()
    {
        var host = new HostBuilder()
            .IncludeConfiguration(TestData.AllTestSettings)
            .UseOptify<DummyClassSettingsB>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();

        await Assert.That(options.Value.X).IsNull();
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Should_allow_required_properties_on_type(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("DummySettingsWithRequiredKeyword:X", value)
            ])
            // This would be a type error if this test is going to fail.
            .UseOptify<DummySettingsWithRequiredKeyword>()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummySettingsWithRequiredKeyword>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }
}