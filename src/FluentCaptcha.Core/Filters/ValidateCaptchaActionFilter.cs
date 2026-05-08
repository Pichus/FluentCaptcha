using FluentCaptcha.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FluentCaptcha.Core.Filters;

public class ValidateCaptchaActionFilter : IAsyncActionFilter
{
    private readonly string _captchaResponseTokenHeaderName;
    private readonly ICaptchaValidator _captchaValidator;

    public ValidateCaptchaActionFilter(
        ICaptchaValidator captchaValidator,
        string captchaResponseTokenHeaderName)
    {
        _captchaValidator = captchaValidator;
        _captchaResponseTokenHeaderName = captchaResponseTokenHeaderName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cancellationToken = context.HttpContext.RequestAborted;

        var requestContainsCaptchaResponseTokenHeader = context.HttpContext.Request.Headers
            .TryGetValue(_captchaResponseTokenHeaderName, out var captchaResponseToken);

        if (!requestContainsCaptchaResponseTokenHeader || string.IsNullOrEmpty(captchaResponseToken.ToString()))
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

        var validationResult = await _captchaValidator.ValidateAsync(
            captchaResponseToken.ToString(),
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
}