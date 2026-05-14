using FluentCaptcha.Core.Attributes;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Core.Extensions;
using FluentCaptcha.Core.Options;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Reflection;

namespace FluentCaptcha.NSwag;

public class ValidateCaptchaFilterOperationProcessor : IOperationProcessor
{
    private readonly FluentCaptchaOptions _fluentCaptchaOptions;

    public ValidateCaptchaFilterOperationProcessor(IOptions<FluentCaptchaOptions> fluentCaptchaOptions)
    {
        _fluentCaptchaOptions = fluentCaptchaOptions.Value;
    }

    public bool Process(OperationProcessorContext context)
    {
        var validateCaptchaAttribute = context.MethodInfo.GetCustomAttribute<ValidateCaptchaAttribute>();

        if (validateCaptchaAttribute is null)
        {
            return true;
        }

        var captchaResponseTokenSource =
            validateCaptchaAttribute.CaptchaResponseTokenSource == CaptchaResponseTokenSource.Default
                ? _fluentCaptchaOptions.DefaultCaptchaResponseTokenSource
                : validateCaptchaAttribute.CaptchaResponseTokenSource;

        var captchaProvider = validateCaptchaAttribute.CaptchaProvider ??
                              _fluentCaptchaOptions.DefaultCaptchaProvider ?? "Unknown";

        switch (captchaResponseTokenSource)
        {
            case CaptchaResponseTokenSource.Default or CaptchaResponseTokenSource.RequestHeader:
                var requestHeaderName = validateCaptchaAttribute.CaptchaResponseTokenRequestHeaderName ??
                                        _fluentCaptchaOptions.DefaultCaptchaResponseTokenRequestHeaderName;

                context.OperationDescription.Operation.Parameters.Add(new OpenApiParameter
                {
                    Name = requestHeaderName,
                    Kind = OpenApiParameterKind.Header,
                    Type = JsonObjectType.String,
                    IsRequired = true,
                    Description = $"Captcha response token header. Captcha provider: {captchaProvider}"
                });

                break;
            case CaptchaResponseTokenSource.RequestBody:
                var parameters = context.MethodInfo.GetParameters();

                foreach (var parameter in parameters)
                {
                    var captchaResponseTokenPropertyName =
                        parameter.ParameterType.FindCaptchaResponseTokenPropertyNameOrDefault();
                    if (captchaResponseTokenPropertyName is not null)
                    {
                        context.OperationDescription.Operation.Description +=
                            $"\n{captchaResponseTokenPropertyName} property is required. You should put your " +
                            $"{captchaProvider} captcha response token here";
                    }
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
    }
}