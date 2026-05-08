using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Exceptions;
using FluentCaptcha.Core.Filters;
using FluentCaptcha.Core.Options;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentCaptcha.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateCaptchaAttribute : Attribute, IFilterFactory
{
    public string? CaptchaProvider { get; set; }
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var fluentCaptchaOptions = serviceProvider.GetService<IOptions<FluentCaptchaOptions>>()?.Value;

        if (fluentCaptchaOptions is null)
        {
            throw new FluentCaptchaConfigurationException("No config provided for fluent captcha.");
        }

        var captchaProviderName = fluentCaptchaOptions.DefaultCaptchaProvider;

        if (CaptchaProvider is not null)
        {
            captchaProviderName = CaptchaProvider;
        }

        var serviceKey = FluentCaptchaConstants.LibraryPrefix + captchaProviderName;
        var captchaProviderInstance = serviceProvider.GetKeyedService<ICaptchaValidator>(serviceKey);

        if (captchaProviderInstance is null)
        {
            throw new FluentCaptchaConfigurationException(
                $"No captcha providers registered with name '{captchaProviderName}'");
        }

        return new ValidateCaptchaActionFilter(captchaProviderInstance);
    }
}