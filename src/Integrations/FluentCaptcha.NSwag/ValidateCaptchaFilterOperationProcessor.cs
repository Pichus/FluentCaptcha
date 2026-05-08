using FluentCaptcha.Core;
using FluentCaptcha.Core.Attributes;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Reflection;

namespace FluentCaptcha.NSwag;

public class ValidateCaptchaFilterOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var validateCaptchaAttribute = context.MethodInfo.GetCustomAttribute<ValidateCaptchaAttribute>();

        if (validateCaptchaAttribute is null)
        {
            return true;
        }

        var requestHeaderName = validateCaptchaAttribute.CaptchaResponseTokenRequestHeaderName ??
                                FluentCaptchaConstants.CaptchaResponseTokenRequestHeaderName;

        context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
        {
            Name = requestHeaderName,
            Kind = OpenApiParameterKind.Header,
            Type = JsonObjectType.String,
            IsRequired = true,
            Description = "Captcha response token header"
        });

        return true;
    }
}