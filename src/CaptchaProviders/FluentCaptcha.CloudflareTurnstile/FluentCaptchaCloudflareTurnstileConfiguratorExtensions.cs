using FluentCaptcha.Core;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCaptcha.CloudflareTurnstile;

public static class FluentCaptchaCloudflareTurnstileConfiguratorExtensions
{
    public static FluentCaptchaConfigurator UseCloudflareTurnstile(
        this FluentCaptchaConfigurator configurator,
        Action<CloudflareTurnstileOptions>? configureOptions = null)
    {
        configurator.AddCloudflareTurnstile(configureOptions);

        configurator.UseCaptchaValidator<CloudflareCaptchaProvider>();
        configurator.UseCaptchaProvider<CloudflareCaptchaProvider>();

        return configurator;
    }

    public static FluentCaptchaConfigurator AddCloudflareTurnstile(
        this FluentCaptchaConfigurator configurator,
        Action<CloudflareTurnstileOptions>? configureOptions = null)
    {
        if (configureOptions is not null)
        {
            configurator.Services.Configure(configureOptions);
        }

        configurator.Services.AddHttpClient<CloudflareTurnstileHttpClient>();

        configurator.AddCaptchaValidator<CloudflareCaptchaValidator>();
        configurator.AddCaptchaProvider<CloudflareCaptchaProvider>();

        return configurator;
    }
}