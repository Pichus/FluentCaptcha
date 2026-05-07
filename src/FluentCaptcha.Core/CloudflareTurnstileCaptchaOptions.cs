using System.ComponentModel.DataAnnotations;

namespace FluentCaptcha.Core;

public class CloudflareTurnstileCaptchaOptions
{
    public static readonly string Position = "CloudflareTurnstileCaptchaOptions";

    [Required] public required string SecretKey { get; set; }

    [Url] public string SiteVerifyUrl { get; } = "https://challenges.cloudflare.com/turnstile/v0/siteverify";
}