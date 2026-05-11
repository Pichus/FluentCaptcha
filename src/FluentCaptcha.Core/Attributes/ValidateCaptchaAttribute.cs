using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Enums;
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
    public string? ExpectedAction { get; set; }

    public CaptchaResponseTokenSource? CaptchaResponseTokenSource { get; set; }

    public string? CaptchaProvider { get; set; }

    public string? CaptchaResponseTokenRequestHeaderName { get; set; }

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var fluentCaptchaOptions = serviceProvider.GetService<IOptions<FluentCaptchaOptions>>()?.Value;

        if (fluentCaptchaOptions is null)
        {
            throw new FluentCaptchaConfigurationException("No config provided for fluent captcha.");
        }

        CaptchaResponseTokenRequestHeaderName ??= FluentCaptchaConstants.CaptchaResponseTokenRequestHeaderName;

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

        var captchaResponseTokenSource =
            CaptchaResponseTokenSource ?? fluentCaptchaOptions.DefaultCaptchaResponseTokenSource;

        return new ValidateCaptchaActionFilter(
            captchaProviderInstance,
            CaptchaResponseTokenRequestHeaderName,
            captchaResponseTokenSource,
            ExpectedAction);
    }
}