namespace FluentCaptcha.CloudflareTurnstile;

public static class CloudflareTurnstileConstants
{
    public static readonly string ApiUrl = "https://challenges.cloudflare.com/turnstile/v0/siteverify";

    public static class TestSecretKeys
    {
        public static readonly string AlwaysPassValidation = "1x0000000000000000000000000000000AA";

        public static readonly string AlwaysFailValidation = "2x0000000000000000000000000000000AA";

        public static readonly string ReturnTokenAlreadySpentError = "2x0000000000000000000000000000000AA";
    }
}