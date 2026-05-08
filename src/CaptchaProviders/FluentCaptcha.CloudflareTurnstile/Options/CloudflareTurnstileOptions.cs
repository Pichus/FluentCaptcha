namespace FluentCaptcha.CloudflareTurnstile.Options;

public class CloudflareTurnstileOptions
{
    public required string SecretKey { get; set; }

    public required string SiteVerifyUrl { get; set; } = CloudflareTurnstileConstants.ApiUrl;

    public int MaxRetryCount { get; set; } = 3;

    public bool RetryOnFailure { get; set; } = false;
}