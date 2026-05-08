using FluentCaptcha.Core;
using FluentCaptcha.Core.Attributes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FluentCaptcha.Swashbuckle;

public class ValidateCaptchaOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var validateCaptchaAttribute = context.MethodInfo
            .GetCustomAttributes(typeof(ValidateCaptchaAttribute), true)
            .FirstOrDefault() as ValidateCaptchaAttribute;

        if (validateCaptchaAttribute is null)
        {
            return;
        }

        var requestHeaderName = validateCaptchaAttribute.CaptchaResponseTokenRequestHeaderName ??
                                FluentCaptchaConstants.CaptchaResponseTokenRequestHeaderName;

        if (operation.Parameters is null)
        {
            operation.Parameters = new List<IOpenApiParameter>();
        }

        operation.Parameters?.Add(new OpenApiParameter
        {
            Name = requestHeaderName,
            In = ParameterLocation.Header,
            Description = "Captcha response token header",
            Required = true
        });
    }
}