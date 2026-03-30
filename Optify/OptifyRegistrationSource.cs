using System.Collections.Immutable;
using Scriban;
using Scriban.Runtime;

namespace Optify;

public static class OptifyRegistrationSource
{
    public const string Filename = "OptifyRegistration.g.cs";

    private static readonly Template SourceTemplate = LoadTemplate();

    public static string GenerateSource(ImmutableArray<OptionsTypeToRegister> optionsToRegister)
    {
        var options = new ScriptArray();

        foreach (var opt in optionsToRegister)
        {
            options.Add(
                new ScriptObject
                {
                    ["full_name"] = opt.FullName,
                    ["section_name"] = opt.SectionName,
                    ["has_data_annotations"] = opt.Validation.HasFlag(ValidationFlag.DataAnnotations),
                    ["has_validate_on_start"] = opt.Validation.HasFlag(ValidationFlag.OnStart),
                }
            );
        }

        var model = new ScriptObject { ["options"] = options };
        var context = new TemplateContext();
        context.PushGlobal(model);

        return SourceTemplate.Render(context);
    }

    private static Template LoadTemplate()
    {
        var assembly = typeof(OptifyRegistrationSource).Assembly;
        using var stream = assembly.GetManifestResourceStream("Optify.OptifyRegistration.sbn-cs");
        using var reader = new StreamReader(stream!);
        return Template.Parse(reader.ReadToEnd());
    }
}
