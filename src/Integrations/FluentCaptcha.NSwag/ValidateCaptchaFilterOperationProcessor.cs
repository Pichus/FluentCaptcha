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
        if (context.MethodInfo.GetCustomAttribute<ValidateCaptchaAttribute>() is null)
        {
            return true;
        }

        context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
        {
            Name = FluentCaptchaConstants.CaptchaResponseTokenRequestHeaderName,
            Kind = OpenApiParameterKind.Header,
            Type = JsonObjectType.String,
            IsRequired = true,
            Description = "Captcha response token header"
        });

        return true;
    }
}