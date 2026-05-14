using FluentCaptcha.Core.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSwag.Generation.AspNetCore;

namespace FluentCaptcha.NSwag;

public static class FluentCaptchaAspNetCoreOpenApiDocumentGeneratorSettingsExtensions
{
    public static AspNetCoreOpenApiDocumentGeneratorSettings IntegrateWithFluentCaptcha(
        this AspNetCoreOpenApiDocumentGeneratorSettings settings, IServiceProvider serviceProvider)
    {
        var fluentCaptchaOptions = serviceProvider.GetRequiredService<IOptions<FluentCaptchaOptions>>();
        settings.OperationProcessors.Add(new ValidateCaptchaFilterOperationProcessor(fluentCaptchaOptions));

        return settings;
    }
}