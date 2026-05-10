using FluentCaptcha.Core.Abstractions;

namespace FluentCaptcha.Dummy;

public class DummyCaptchaValidator : ICaptchaValidator
{
    public async Task<CaptchaValidationResult> ValidateAsync(
        string captchaResponseToken,
        string? remoteIp = null,
        string? expectedAction = null,
        CancellationToken cancellationToken = default)
    {
        if (expectedAction is not null)
        {
            throw new NotSupportedException("Validating the action is not supported by Dummy captcha validator.");
        }

        return CaptchaValidationResult.Success();
    }
}