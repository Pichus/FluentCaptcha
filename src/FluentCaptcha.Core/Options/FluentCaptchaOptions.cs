using FluentCaptcha.Core.Enums;

namespace FluentCaptcha.Core.Options;

public class FluentCaptchaOptions
{
    public CaptchaResponseTokenSource DefaultCaptchaResponseTokenSource { get; set; }

    public string? DefaultCaptchaProvider { get; set; }
}