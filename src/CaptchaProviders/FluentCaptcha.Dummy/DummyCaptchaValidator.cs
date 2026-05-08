using FluentCaptcha.Core.Abstractions;

namespace FluentCaptcha.Dummy;

public class DummyCaptchaValidator : ICaptchaValidator
{
    public async Task<CaptchaValidationResult> ValidateAsync(string captchaResponseToken, string? remoteIp = null)
    {
        return CaptchaValidationResult.Success();
    }
}