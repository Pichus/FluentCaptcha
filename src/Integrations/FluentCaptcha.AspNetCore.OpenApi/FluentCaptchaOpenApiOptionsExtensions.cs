using Microsoft.AspNetCore.OpenApi;

namespace FluentCaptcha.AspNetCore.OpenApi;

public static class FluentCaptchaOpenApiOptionsExtensions
{
    public static OpenApiOptions IntegrateWithFluentCaptcha(this OpenApiOptions options)
    {
        options.AddOperationTransformer<ValidateCaptchaOperationTransformer>();
        return options;
    }
}