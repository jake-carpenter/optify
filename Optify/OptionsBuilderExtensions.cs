using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Optify;

public static class OptionsBuilderExtensions
{
    public static void MaybeAddValidation<T>(this OptionsBuilder<T> builder, ValidationFlag validation)
        where T : class
    {
        var validateAnnotations = validation.HasFlag(ValidationFlag.DataAnnotations);
        var validateOnStart = validation.HasFlag(ValidationFlag.OnStart);

        if (!validateAnnotations && !validateOnStart)
            return;

        if (validateAnnotations)
        {
            builder.ValidateDataAnnotations();
        }

        if (validateOnStart)
        {
            builder.ValidateOnStart();
        }
    }
}