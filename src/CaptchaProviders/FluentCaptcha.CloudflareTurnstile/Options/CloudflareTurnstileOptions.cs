namespace FluentCaptcha.CloudflareTurnstile.Options;

public class CloudflareTurnstileOptions
{
    public required string SecretKey { get; set; }

    public required string SiteVerifyUrl { get; set; } = CloudflareTurnstileConstants.ApiUrl;
}