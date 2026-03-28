using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Optify;

public static class OptionsBuilderExtensions
{
    public static OptionsBuilder<T> MaybeAddValidation<T>(this OptionsBuilder<T> builder, ValidationFlag validation)
        where T : class
    {
        var validateAnnotations = validation.HasFlag(ValidationFlag.DataAnnotations);
        var validateOnStart = validation.HasFlag(ValidationFlag.OnStart);

        if (validateAnnotations)
        {
            builder.ValidateDataAnnotations();
        }

        if (validateOnStart)
        {
            builder.ValidateOnStart();
        }

        return builder;
    }
}