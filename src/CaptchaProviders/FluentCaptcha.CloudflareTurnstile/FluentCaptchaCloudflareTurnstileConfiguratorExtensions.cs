using FluentCaptcha.CloudflareTurnstile.Options;
using FluentCaptcha.Core;

namespace FluentCaptcha.CloudflareTurnstile;

public static class FluentCaptchaCloudflareTurnstileConfiguratorExtensions
{
    public static IFluentCaptchaConfigurator UseCloudflareTurnstile(
        this IFluentCaptchaConfigurator configurator,
        Action<CloudflareTurnstileOptions>? configureOptions = null)
    {
        configurator.AddCaptchaProvider<CloudflareCaptchaValidator>();

        void DefaultOptionsConfig(CloudflareTurnstileOptions options)
        {
            options.SecretKey = CloudflareTurnstileConstants.TestSecretKeys.AlwaysPassValidation;
        }

        configurator.AddOptions(configureOptions ?? DefaultOptionsConfig);

        return configurator;
    }
}