using FluentCaptcha.Core.Abstractions;

namespace FluentCaptcha.CloudflareTurnstile;

public class CloudflareCaptchaProvider : ICaptchaProvider
{
    private readonly CloudflareCaptchaValidator _captchaValidator;

    public CloudflareCaptchaProvider(CloudflareCaptchaValidator captchaValidator)
    {
        _captchaValidator = captchaValidator;
    }

    public static string Name => CloudflareTurnstileConstants.CaptchaProviderName;

    public string CaptchaResponseTokenFormParameterName =>
        CloudflareTurnstileConstants.CaptchaResponseTokenFormParameterName;

    public async Task<CaptchaValidationResult> ValidateAsync(
        string captchaResponseToken,
        string? remoteIp = null,
        string? expectedAction = null,
        CancellationToken cancellationToken = default)
    {
        return await _captchaValidator.ValidateAsync(captchaResponseToken, remoteIp, expectedAction, cancellationToken);
    }
}