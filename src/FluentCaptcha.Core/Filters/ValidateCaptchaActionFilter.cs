using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Attributes;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace FluentCaptcha.Core.Filters;

public class ValidateCaptchaActionFilter : IAsyncActionFilter
{
    private readonly string _captchaResponseTokenHeaderName;
    private readonly CaptchaResponseTokenSource _captchaResponseTokenSource;
    private readonly ICaptchaValidator _captchaValidator;

    public ValidateCaptchaActionFilter(
        ICaptchaValidator captchaValidator,
        string captchaResponseTokenHeaderName,
        CaptchaResponseTokenSource captchaResponseTokenSource)
    {
        _captchaValidator = captchaValidator;
        _captchaResponseTokenHeaderName = captchaResponseTokenHeaderName;
        _captchaResponseTokenSource = captchaResponseTokenSource;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cancellationToken = context.HttpContext.RequestAborted;

        var captchaResponseToken = "";

        switch (_captchaResponseTokenSource)
        {
            case CaptchaResponseTokenSource.RequestHeader:
                var requestContainsCaptchaResponseTokenHeader = context.HttpContext.Request.Headers
                    .TryGetValue(_captchaResponseTokenHeaderName, out var captchaResponseTokenRequestHeader);

                var captchaResponseTokenRequestHeaderValue = captchaResponseTokenRequestHeader.ToString();

                if (!requestContainsCaptchaResponseTokenHeader ||
                    string.IsNullOrEmpty(captchaResponseTokenRequestHeaderValue))
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Missing required header",
                        Status = StatusCodes.Status400BadRequest,
                        Detail = $"{_captchaResponseTokenHeaderName} request header is required for this endpoint"
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                    return;
                }

                captchaResponseToken = captchaResponseTokenRequestHeaderValue;

                break;
            case CaptchaResponseTokenSource.RequestBody:
                var captchaResponseTokenFromBody = FindCaptchaResponseTokenInActionArguments(context.ActionArguments);

                if (captchaResponseTokenFromBody is null)
                {
                    throw new FluentCaptchaErrorException(
                        "No field with CaptchaResponseToken attribute applied found in request body");
                }

                captchaResponseToken = captchaResponseTokenFromBody;

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        var validationResult = await _captchaValidator.ValidateAsync(
            captchaResponseToken,
            cancellationToken: cancellationToken);

        if (validationResult.IsFailure)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Captcha validation failed.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Captcha validation failed."
            };
            context.Result = new BadRequestObjectResult(problemDetails);
            return;
        }

        context.HttpContext.Items[FluentCaptchaConstants.ValidationResultHttpContextItemsKey] = validationResult;

        await next();
    }

    private string? FindCaptchaResponseTokenInActionArguments(IDictionary<string, object> actionArguments)
    {
        string? captchaResponseToken = null;
        foreach (var (argumentName, argumentValue) in actionArguments)
        {
            var argumentType = argumentValue.GetType();
            var argumentCaptchaResponseTokenAttribute =
                argumentType.GetCustomAttribute<CaptchaResponseTokenAttribute>();

            if (argumentCaptchaResponseTokenAttribute is not null && argumentValue is string argumentValueString)
            {
                captchaResponseToken = argumentValueString;
                break;
            }

            var argumentFields = argumentType.GetProperties();

            foreach (var argumentField in argumentFields)
            {
                var argumentFieldCaptchaResponseTokenAttribute =
                    argumentField.GetCustomAttribute<CaptchaResponseTokenAttribute>();

                if (argumentFieldCaptchaResponseTokenAttribute is null ||
                    argumentField.GetValue(argumentValue) is not string argumentFieldValue)
                {
                    continue;
                }

                captchaResponseToken = argumentFieldValue;
                break;
            }
        }

        return captchaResponseToken;
    }
}