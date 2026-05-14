namespace FluentCaptcha.Core;

public static class FluentCaptchaConstants
{
    public const string LibraryPrefix = "FluentCaptcha__";

    public static readonly string ValidationResultHttpContextItemsKey = LibraryPrefix + "ValidationResult";

    public static readonly string DefaultCaptchaResponseTokenRequestHeaderName = "X-Captcha-Response";
}