using FluentCaptcha.CloudflareTurnstile.Options;
using FluentCaptcha.Core;

namespace FluentCaptcha.CloudflareTurnstile;

public static class FluentCaptchaCloudflareTurnstileConfiguratorExtensions
{
    public static FluentCaptchaConfigurator UseCloudflareTurnstile(
        this FluentCaptchaConfigurator configurator,
        Action<CloudflareTurnstileOptions>? configureOptions = null)
    {
        configurator.AddCloudflareTurnstile(configureOptions);
        configurator.DefaultCaptchaProvider = CloudflareTurnstileConstants.CaptchaProviderName;

        return configurator;
    }

    public static FluentCaptchaConfigurator AddCloudflareTurnstile(
        this FluentCaptchaConfigurator configurator,
        Action<CloudflareTurnstileOptions>? configureOptions = null)
    {
        configurator.AddCaptchaProvider<CloudflareCaptchaValidator>(
            CloudflareTurnstileConstants.CaptchaProviderName,
            true);

        void DefaultOptionsConfig(CloudflareTurnstileOptions options)
        {
            options.SecretKey = CloudflareTurnstileConstants.TestSecretKeys.AlwaysPassValidation;
        }

        configurator.AddOptions(configureOptions ?? DefaultOptionsConfig);

        return configurator;
    }
}