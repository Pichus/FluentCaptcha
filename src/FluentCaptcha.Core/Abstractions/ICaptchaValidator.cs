namespace FluentCaptcha.Core.Abstractions;

public interface ICaptchaValidator
{
    Task<CaptchaValidationResult> ValidateAsync(string captchaResponseToken, string? remoteIp = null);
}