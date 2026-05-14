using FluentCaptcha.Core.Attributes;
using System.Reflection;

namespace FluentCaptcha.Core.Extensions;

public static class FluentCaptchaObjectExtensions
{
    public static bool TryGetCaptchaResponseTokenPropertyValue(this object argumentValue,
        out string captchaResponseToken)
    {
        var result = false;
        captchaResponseToken = "";

        var argumentType = argumentValue.GetType();

        var argumentProperties = argumentType.GetProperties();

        foreach (var argumentProperty in argumentProperties)
        {
            var argumentPropertyCaptchaResponseTokenAttribute =
                argumentProperty.GetCustomAttribute<CaptchaResponseTokenAttribute>();

            if (argumentPropertyCaptchaResponseTokenAttribute is null ||
                argumentProperty.GetValue(argumentValue) is not string captchaResponseTokenString)
            {
                continue;
            }

            result = true;
            captchaResponseToken = captchaResponseTokenString;
            break;
        }

        return result;
    }
}