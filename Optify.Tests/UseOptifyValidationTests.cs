using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Optify.Tests;

public class UseOptifyValidationTests
{
    [Test]
    public async Task Validates_data_annotations_when_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("ValidatedDummySettings:X", null)
            ])
            .UseOptify(ValidationFlag.DataAnnotations)
            .Build();

        var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();

        await Assert.That(() => options.Value).Throws<OptionsValidationException>();
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Validation_does_not_throw_when_data_annotations_are_satisfied_from_extension(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("ValidatedDummySettings:X", value)
            ])
            .UseOptify(ValidationFlag.DataAnnotations)
            .Build();

        var options = host.Services.GetRequiredService<IOptions<ValidatedDummySettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    public async Task Validates_on_start_when_configured_from_extension()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
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
                ..TestData.AllTestSettings,
                new("ValidatedDummySettings:X", null),
                new("AttrValidatedOnStartDummySettings:X", "arbitrary")
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
                ..TestData.AllTestSettings,
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
                ..TestData.AllTestSettings,
                new("AttrValidatedOnStartDummySettings:X", null)
            ])
            .UseOptify(ValidationFlag.DataAnnotations | ValidationFlag.OnStart)
            .Build();

        await Assert.That(async () => await host.StartAsync()).Throws<OptionsValidationException>();
    }

    [Test]
    [MethodDataSource<TestData>(nameof(TestData.GenericStringTestData))]
    public async Task Validation_does_not_throw_when_data_annotations_are_satisfied_from_attribute(string value)
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
                new("AttrValidatedDummySettings:X", value)
            ])
            .UseOptify()
            .Build();

        var options = host.Services.GetRequiredService<IOptions<AttrValidatedDummySettings>>();

        await Assert.That(options.Value.X).IsEqualTo(value);
    }

    [Test]
    public async Task Validates_once_created_when_validate_on_start_not_configured_from_attribute()
    {
        var host = new HostBuilder()
            .IncludeConfiguration([
                ..TestData.AllTestSettings,
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