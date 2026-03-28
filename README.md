# Optify

**Optify** is a Roslyn source generator that
wires [options pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options) types into the generic host:
mark a class or record with `[OptifyOptions]`,
call
`UseOptify()` on your `IHostBuilder`, and each type will automatically be bound to configuration based on the type name
or the `SectionName` attribute.

## Installation

```bash
dotnet add package Optify
```

## Recommended usage

Mark your configuration types with `[OptifyOptions]` so registration code is generated at compile time.

**IMPORTANT**: Types **without** `[OptifyOptions]` are not registered by `UseOptify()`.

### 1. Mark options types

Apply `[OptifyOptions]` to types that represent configuration.

```csharp
using Optify;

// Binds to section "MyAppSettings" by convention - matches class name.
[OptifyOptions]
public class MyAppSettings
{
    public string? ApiUrl { get; init; }
}

// Binds to section "ExternalServices" - specified by attribute.
[Optify(SectionName = "ExternalServices")]
public record EmailSettings
{
    public string? SmtpHost { get; init; }
}
```

### 2. Register with the host

**Register every `[OptifyOptions]` type in the current assembly** (recommended; registration is emitted as concrete
calls—no
reflection):

```csharp
using Optify;

var host = Host.CreateDefaultBuilder(args)
    // ...
    .UseOptify()
    // ...
    .Build();
```

With **ASP.NET Core** minimal hosting, call it on `WebApplicationBuilder.Host`:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOptify();
```

## Alternatively: register a single type

Useful for testing scenarios when you don't want to register all types in the assembly. Avoid this if you have control
over your types because this requires reflection.

```csharp
// No [OptifyOptions] attribute required to register by convention.
public class MyAppSettings
{
    public string? ApiUrl { get; init; }
}

// Use attribute to specify a different section name.
[Optify(SectionName = "ExternalServices")]
public record EmailSettings
{
    public string? SmtpHost { get; init; }
}

var host = Host.CreateDefaultBuilder(args)
    // ...
    .UseOptify<MyAppSettings>()
    .UseOptify<EmailSettings>()
    // ...
    .Build();
```

## Validation

Optify
supports [options validation](https://learn.microsoft.com/en-us/dotnet/core/extensions/options#options-validation) via
the `ValidationFlag` enum:

| Flag              | Effect                                                              |
|-------------------|---------------------------------------------------------------------|
| `DataAnnotations` | Validates using `System.ComponentModel.DataAnnotations` attributes. |
| `OnStart`         | Validates eagerly at host startup rather than on first access.      |

Flags can be combined with `|`. There are two ways to configure validation:

### Per-attribute

Set the `Validation` property on `[OptifyOptions]`. This works with both `UseOptify()` and `UseOptify<T>()`.

```csharp
using System.ComponentModel.DataAnnotations;
using Optify;

[OptifyOptions(Validation = ValidationFlag.DataAnnotations | ValidationFlag.OnStart)]
public class MyAppSettings
{
    [Required]
    public string ApiUrl { get; init; } = null!;
}
```

### Per-registration

Pass a `ValidationFlag` to `UseOptify()` or an `OptifyConfiguration` to `UseOptify<T>()` to apply validation to all
registered types.

```csharp
// Source-generated: applies to all [OptifyOptions] types in the assembly.
var host = Host.CreateDefaultBuilder(args)
    .UseOptify(ValidationFlag.DataAnnotations | ValidationFlag.OnStart)
    .Build();

// Single type: applies only to MyAppSettings.
var host = Host.CreateDefaultBuilder(args)
    .UseOptify<MyAppSettings>(new OptifyConfiguration
    {
        Validation = ValidationFlag.DataAnnotations | ValidationFlag.OnStart
    })
    .Build();
```

Flags from the attribute and the registration call are additive — if either specifies `DataAnnotations`, data annotation
validation is enabled for that type.

## Section name resolution

When using `UseOptify<T>()`, the configuration section name is resolved in the following order of precedence:

1. `OptifyConfiguration.SectionName` — if provided to the extension method.
2. `[OptifyOptions(SectionName = "...")]` — if set on the type's attribute.
3. The type name (e.g. `MyAppSettings` binds to section `"MyAppSettings"`).

When using the source-generated `UseOptify()`, only steps 2 and 3 apply since there is no `OptifyConfiguration`
parameter.

## Configuration layout

Match your `appsettings.json` (or other configuration sources) to the resolved section names:

```json
{
  "MyAppSettings": {
    "ApiUrl": "https://api.example.com"
  },
  "ExternalServices": {
    "SmtpHost": "smtp.example.com"
  }
}
```

## License

This project is licensed under the MIT License—see [LICENSE.md](LICENSE.md).
