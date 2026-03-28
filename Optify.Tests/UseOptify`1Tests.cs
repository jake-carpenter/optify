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

    [Test]
    public async Task Registers_section_name_from_extension_config_as_first_priority()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
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
                ..TestHelpers.AllTestSettings,
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
                ..TestHelpers.AllTestSettings,
                new("DummyClassSettingsA:X", "came-from-type-name")
            ])
            .UseOptify<DummyClassSettingsA>(new OptifyConfiguration()) // no SectionName, type has no attribute SectionName
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
            .UseOptify<ValidatedDummySettings>(new OptifyConfiguration { Validation = ValidationFlag.DataAnnotations })
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
            .UseOptify<ValidatedDummySettings>(new OptifyConfiguration { Validation = ValidationFlag.DataAnnotations })
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
            .UseOptify<ValidatedDummySettings>(new OptifyConfiguration
            {
                Validation = ValidationFlag.DataAnnotations | ValidationFlag.OnStart
            })
            .Build();

        var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();

        await Assert.That(() => host.StartAsync()).Throws<OptionsValidationException>();
    }

    [Test]
    public async Task Validates_when_created_when_validate_on_start_not_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestHelpers.AllTestSettings,
                new("ValidatedDummySettings:X", null)
            ])
            .UseOptify<ValidatedDummySettings>(new OptifyConfiguration { Validation = ValidationFlag.DataAnnotations })
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
            .UseOptify<AttrValidatedDummySettings>()
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
            .UseOptify<AttrValidatedOnStartDummySettings>()
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
            .UseOptify<AttrValidatedDummySettings>()
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
            .UseOptify<AttrValidatedDummySettings>()
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