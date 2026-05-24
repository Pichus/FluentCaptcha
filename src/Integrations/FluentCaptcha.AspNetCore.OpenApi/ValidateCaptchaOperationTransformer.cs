using FluentCaptcha.Core.Attributes;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Core.Extensions;
using FluentCaptcha.Core.Options;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace FluentCaptcha.AspNetCore.OpenApi;

public class ValidateCaptchaOperationTransformer : IOpenApiOperationTransformer
{
    private readonly FluentCaptchaOptions _fluentCaptchaOptions;

    public ValidateCaptchaOperationTransformer(IOptions<FluentCaptchaOptions> fluentCaptchaOptions)
    {
        _fluentCaptchaOptions = fluentCaptchaOptions.Value;
    }

    public async Task TransformAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var validateCaptchaAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<ValidateCaptchaAttribute>()
            .FirstOrDefault();

        if (validateCaptchaAttribute is null)
        {
            return;
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

                operation.Parameters ??= new List<IOpenApiParameter>();

                operation.Parameters?.Add(new OpenApiParameter
                {
                    Name = requestHeaderName,
                    In = ParameterLocation.Header,
                    Description = $"Captcha response token header. Captcha provider: {captchaProvider}",
                    Required = true
                });

                break;
            case CaptchaResponseTokenSource.RequestBody:
                var parameters = context.Description.ActionDescriptor.Parameters;

                foreach (var parameter in parameters)
                {
                    var captchaResponseTokenPropertyName =
                        parameter.ParameterType.FindCaptchaResponseTokenPropertyNameOrDefault();
                    if (captchaResponseTokenPropertyName is not null)
                    {
                        operation.Description +=
                            $"\n{captchaResponseTokenPropertyName} property represents the captcha response token. " +
                            $"Captcha provider: {captchaProvider}";
                    }
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}