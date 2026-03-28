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
                ..TestHelpers.AllTestSettings,
                new("DummyClassSettingsA:X", "one"),
                new("DummyClassSettingsB:X", "two")
            ])
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
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("DummyRecordSettings:X", "one")
            ])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummyRecordSettings>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    [Arguments("foo")]
    [Arguments("bar")]
    public async Task Registers_marked_class_with_provided_name(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("OverrideNamedDummyClassSettings:X", value)
            ])
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
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("OverrideNamedDummyRecordSettings:X", value)
            ])
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
                ..TestHelpers.AllTestSettings,
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
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("DummySettingsWithRequiredKeyword:X", "one")
            ])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<DummySettingsWithRequiredKeyword>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

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

    [Test]
    public async Task Validates_data_annotations_when_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("ValidatedDummySettings:X", null)
            ])
            .UseOptify(ValidationFlag.DataAnnotations)
            .Build();

        var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();

        await Assert.That(() => options.Value).Throws<OptionsValidationException>();
    }

    [Test]
    public async Task Validation_does_not_throw_when_data_annotations_are_satisfied_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("ValidatedDummySettings:X", "one")
            ])
            .UseOptify(ValidationFlag.DataAnnotations)
            .Build();

        var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    public async Task Validates_on_start_when_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("ValidatedDummySettings:X", null)
            ])
            .UseOptify(ValidationFlag.DataAnnotations | ValidationFlag.OnStart)
            .Build();

        await Assert.That(() => host.StartAsync()).Throws<OptionsValidationException>();
    }

    [Test]
    public async Task Validates_once_created_when_validate_on_start_not_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("ValidatedDummySettings:X", null),
                new("AttrValidatedOnStartDummySettings:X", "one")
            ])
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

    [Test]
    public async Task Validates_data_annotations_when_configured_from_attribute()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("AttrValidatedDummySettings:X", null)
            ])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<AttrValidatedDummySettings>>();

        await Assert.That(() => options.Value).Throws<OptionsValidationException>();
    }

    [Test]
    public async Task Validates_on_start_when_configured_with_attribute()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("AttrValidatedOnStartDummySettings:X", null)
            ])
            .UseOptify(ValidationFlag.DataAnnotations | ValidationFlag.OnStart)
            .Build();

        await Assert.That(async () => await host.StartAsync()).Throws<OptionsValidationException>();
    }

    [Test]
    public async Task Validation_does_not_throw_when_data_annotations_are_satisfied_from_attribute()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("AttrValidatedDummySettings:X", "one")
            ])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<AttrValidatedDummySettings>>();

        await Assert.That(options.Value.X).IsEqualTo("one");
    }

    [Test]
    public async Task Validates_once_created_when_validate_on_start_not_configured_from_attribute()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("AttrValidatedDummySettings:X", null)
            ])
            .UseOptify()
            .Build();

        await Assert.That(() => host.StartAsync()).ThrowsNothing();

        await Assert
            .That(() =>
            {
                var options = host.Services.GetRequiredService<IOptions<AttrValidatedDummySettings>>();
                return options.Value; // Have to access 'Value' to trigger validation.
            })
            .Throws<OptionsValidationException>();
    }
}