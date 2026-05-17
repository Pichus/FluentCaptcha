using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Core.Exceptions;
using FluentCaptcha.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FluentCaptcha.Core.Filters;

public class ValidateCaptchaActionFilter : IAsyncActionFilter
{
    private readonly string? _captchaResponseTokenFormParameterName;
    private readonly string _captchaResponseTokenHeaderName;
    private readonly CaptchaResponseTokenSource _captchaResponseTokenSource;
    private readonly ICaptchaValidator _captchaValidator;
    private readonly string? _expectedAction;

    public ValidateCaptchaActionFilter(
        ICaptchaValidator captchaValidator,
        string captchaResponseTokenHeaderName,
        CaptchaResponseTokenSource captchaResponseTokenSource,
        string? captchaResponseTokenFormParameterName = null,
        string? expectedAction = null)
    {
        _captchaValidator = captchaValidator;
        _captchaResponseTokenHeaderName = captchaResponseTokenHeaderName;
        _captchaResponseTokenSource = captchaResponseTokenSource;
        _captchaResponseTokenFormParameterName = captchaResponseTokenFormParameterName;
        _expectedAction = expectedAction;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cancellationToken = context.HttpContext.RequestAborted;

        var remoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString();

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
            case CaptchaResponseTokenSource.RequestForm:
                if (_captchaResponseTokenFormParameterName is null)
                {
                    throw new NotSupportedException(
                        "Selected captcha provider does not support captcha response token extraction from form parameters.");
                }

                var formContainsCaptchaResponseTokenParameter = context.HttpContext.Request.Form.TryGetValue(
                    _captchaResponseTokenFormParameterName,
                    out var captchaResponseTokenFromForm);

                if (!formContainsCaptchaResponseTokenParameter ||
                    string.IsNullOrWhiteSpace(captchaResponseTokenFromForm))
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Missing required form parameter",
                        Status = StatusCodes.Status400BadRequest,
                        Detail =
                            $"{_captchaResponseTokenFormParameterName} form parameter is required for this endpoint"
                    };
                    context.Result = new BadRequestObjectResult(problemDetails);
                    return;
                }

                captchaResponseToken = captchaResponseTokenFromForm.ToString();

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        var validationResult = await _captchaValidator.ValidateAsync(
            captchaResponseToken,
            remoteIp,
            _expectedAction,
            cancellationToken);

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

    private static string? FindCaptchaResponseTokenInActionArguments(IDictionary<string, object> actionArguments)
    {
        foreach (var actionArgument in actionArguments)
        {
            var foundToken = actionArgument.Value.TryGetCaptchaResponseTokenPropertyValue(out var captchaResponseToken);
            if (foundToken)
            {
                return captchaResponseToken;
            }
        }

        return null;
    }
}