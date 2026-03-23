using ExampleApp;
using Microsoft.Extensions.Options;
using Optify;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOptify();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet(
    "/",
    (IOptionsSnapshot<Settings> options, IOptionsSnapshot<ServiceSettings> serviceOptions) =>
    {
        return Results.Ok(new { options.Value.MagicValue, serviceOptions.Value.BaseUrl });
    });

app.Run();