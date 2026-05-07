using FluentCaptcha.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FluentCaptcha.Core;

public class ValidateCaptchaActionFilter : IAsyncActionFilter
{
    private readonly ICaptchaValidator _captchaValidator;

    public ValidateCaptchaActionFilter(ICaptchaValidator captchaValidator)
    {
        _captchaValidator = captchaValidator;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var requestContainsCaptchaResponseTokenHeader = context.HttpContext.Request.Headers
            .TryGetValue("X-Captcha-Response", out var captchaResponseToken);
        
        if (!requestContainsCaptchaResponseTokenHeader || string.IsNullOrEmpty(captchaResponseToken.ToString()))
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Missing required header",
                Status = StatusCodes.Status400BadRequest,
                Detail = "X-Captcha-Response request header is required for this endpoint",
            };
            context.Result = new BadRequestObjectResult(problemDetails);
            return;
        }
        
        var validationResult = await _captchaValidator.ValidateAsync(captchaResponseToken.ToString());
        
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
        
        await next();
    }
}