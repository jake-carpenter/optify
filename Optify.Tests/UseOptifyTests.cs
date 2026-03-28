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
            .IncludeConfiguration(new("DummyClassSettingsA:X", "one"), new("DummyClassSettingsB:X", "two"))
            .UseOptify()
            .Build();

        var optionsA = host.Services.GetRequiredService<IOptions<DummyClassSettingsA>>();
        var optionsB = host.Services.GetRequiredService<IOptions<DummyClassSettingsB>>();

        using var _ = Assert.Multiple();
        await Assert.That(optionsA.Value.X).IsEqualTo("one");
        await Assert.That(optionsB.Value.X).IsEqualTo("two");
    }

    [Test]
    public async Task Registers_all_marked_records()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("DummyRecordSettings:X", "one")])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyRecordSettings>>();

        using var _ = Assert.Multiple();
        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    [Arguments("foo")]
    [Arguments("bar")]
    public async Task Registers_marked_class_with_provided_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("OverrideNamedDummyClassSettings:X", value)])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyClassSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    [Arguments("foo")]
    [Arguments("bar")]
    public async Task Registers_marked_record_with_provided_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("OverrideNamedDummyRecordSettings:X", value)])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<NamedDummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    public async Task Should_not_register_unmarked_types()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                new("DummyClassSettingsA:X", "one"),
                new("DummyClassSettingsB:X", "two"),
                new("UnmarkedDummyClassSettings:Setting", "three")
            ])
            .UseOptify()
            .Build();

        var options = host.Services.GetService<IOptions<UnmarkedDummyClassSettings>>();

        await Assert.That(options!.Value.X).IsNull();
    }

    [Test]
    public async Task Should_allow_required_properties_on_type()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("DummySettingsWithRequiredKeyword:X", "one")])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummySettingsWithRequiredKeyword>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    public async Task Validates_data_annotations_when_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("ValidatedDummySettings:X", null)])
            .UseOptify(ValidationFlag.DataAnnotations)
            .Build();

        var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();

        await Assert.That(() => options.Value).Throws<OptionsValidationException>();
    }

    [Test]
    public async Task Validation_does_not_throw_when_data_annotations_are_satisfied_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("ValidatedDummySettings:X", "one")])
            .UseOptify(ValidationFlag.DataAnnotations)
            .Build();

        var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    public async Task Validates_on_start_when_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("ValidatedDummySettings:X", null)])
            .UseOptify(ValidationFlag.DataAnnotations | ValidationFlag.OnStart)
            .Build();

        var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();

        await Assert.That(() => host.StartAsync()).Throws<OptionsValidationException>();
    }

    [Test]
    public async Task Validates_when_created_when_validate_on_start_not_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([new("ValidatedDummySettings:X", null)])
            .UseOptify(ValidationFlag.DataAnnotations)
            .Build();

        await Assert.That(() => host.StartAsync()).ThrowsNothing();

        await Assert
            .That(() =>
            {
                var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();
                return options.Value; // Have to access 'Value' to trigger validation.
            })
            .Throws<OptionsValidationException>();
    }
}