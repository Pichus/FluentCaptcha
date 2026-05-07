using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCaptcha.Core;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateCaptchaAttribute : TypeFilterAttribute
{
    public ValidateCaptchaAttribute()
        : base(typeof(ValidateCaptchaActionFilter))
    {
        IsReusable = false;
    }
}