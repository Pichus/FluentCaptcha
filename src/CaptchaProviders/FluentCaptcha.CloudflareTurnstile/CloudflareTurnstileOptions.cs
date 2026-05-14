namespace FluentCaptcha.CloudflareTurnstile;

public class CloudflareTurnstileOptions
{
    public required string SecretKey { get; set; } = CloudflareTurnstileConstants.TestSecretKeys.AlwaysFailValidation;

    public required string SiteVerifyUrl { get; set; } = CloudflareTurnstileConstants.ApiUrl;

    public int MaxRetryCount { get; set; } = 3;

    public bool RetryOnFailure { get; set; } = false;
}