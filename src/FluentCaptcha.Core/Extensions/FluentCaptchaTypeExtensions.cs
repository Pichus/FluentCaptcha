using FluentCaptcha.Core.Attributes;
using System.Reflection;

namespace FluentCaptcha.Core.Extensions;

public static class FluentCaptchaTypeExtensions
{
    public static string? FindCaptchaResponseTokenPropertyNameOrDefault(this Type type)
    {
        var typeProperties = type.GetProperties();

        foreach (var typeProperty in typeProperties)
        {
            var argumentPropertyCaptchaResponseTokenAttribute =
                typeProperty.GetCustomAttribute<CaptchaResponseTokenAttribute>();

            if (argumentPropertyCaptchaResponseTokenAttribute is not null)
            {
                return typeProperty.Name;
            }
        }

        return null;
    }
}