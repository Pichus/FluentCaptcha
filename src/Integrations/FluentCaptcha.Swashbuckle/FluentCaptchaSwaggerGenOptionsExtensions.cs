using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FluentCaptcha.Swashbuckle;

public static class FluentCaptchaSwaggerGenOptionsExtensions
{
    public static SwaggerGenOptions IntegrateWithFluentCaptcha(this SwaggerGenOptions options)
    {
        options.OperationFilter<ValidateCaptchaOperationFilter>();
        return options;
    }
}