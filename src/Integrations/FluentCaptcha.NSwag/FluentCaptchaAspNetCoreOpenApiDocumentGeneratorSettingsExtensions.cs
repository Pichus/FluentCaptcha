using NSwag.Generation.AspNetCore;

namespace FluentCaptcha.NSwag;

public static class FluentCaptchaAspNetCoreOpenApiDocumentGeneratorSettingsExtensions
{
    public static AspNetCoreOpenApiDocumentGeneratorSettings IntegrateWithFluentCaptcha(
        this AspNetCoreOpenApiDocumentGeneratorSettings settings)
    {
        settings.OperationProcessors.Add(new ValidateCaptchaFilterOperationProcessor());
        return settings;
    }
}