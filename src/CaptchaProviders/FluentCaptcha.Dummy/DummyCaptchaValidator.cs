using FluentCaptcha.Core.Abstractions;

namespace FluentCaptcha.Dummy;

public class DummyCaptchaValidator : ICaptchaValidator
{
    public async Task<CaptchaValidationResult> ValidateAsync(
        string captchaResponseToken,
        string? remoteIp = null,
        CancellationToken cancellationToken = default)
    {
        return CaptchaValidationResult.Success();
    }
}