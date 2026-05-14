using FluentCaptcha.Core.Enums;

namespace FluentCaptcha.Core.Options;

public class FluentCaptchaOptions
{
    public CaptchaResponseTokenSource DefaultCaptchaResponseTokenSource { get; set; }

    public required string DefaultCaptchaProvider { get; set; }

    public string DefaultCaptchaResponseTokenRequestHeaderName { get; set; } =
        FluentCaptchaConstants.DefaultCaptchaResponseTokenRequestHeaderName;
}