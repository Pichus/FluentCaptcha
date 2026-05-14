using FluentCaptcha.Core.Abstractions;

namespace FluentCaptcha.Dummy;

public class DummyCaptchaProvider : ICaptchaProvider
{
    private readonly DummyCaptchaValidator _captchaValidator;

    public DummyCaptchaProvider(DummyCaptchaValidator captchaValidator)
    {
        _captchaValidator = captchaValidator;
    }

    public static string Name => DummyConstants.CaptchaProviderName;

    public async Task<CaptchaValidationResult> ValidateAsync(
        string captchaResponseToken,
        string? remoteIp = null,
        string? expectedAction = null,
        CancellationToken cancellationToken = default)
    {
        return await _captchaValidator.ValidateAsync(captchaResponseToken, remoteIp, expectedAction, cancellationToken);
    }
}