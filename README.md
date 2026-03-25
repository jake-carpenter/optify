# Optify

**Optify** is a Roslyn source generator that wires [options pattern](https://learn.microsoft.com/en-us/dotnet/core/extensions/options) types into the generic host: mark a class or record with `[OptifyOptions]`, 
call 
`UseOptify()` on your `IHostBuilder`, and each type will automatically be bound to configuration based on the type name or the `SectionName` attribute.

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

Useful for testing scenarios when you don't want to register all types in the assembly.

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

## Configuration layout

Match your `appsettings.json` (or other configuration sources) to the section names—either the type name or `SectionName` when you set it:

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
