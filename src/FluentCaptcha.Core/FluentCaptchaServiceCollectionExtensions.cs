using FluentCaptcha.Core.Options;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCaptcha.Core;

public static class FluentCaptchaServiceCollectionExtensions
{
    public static IServiceCollection AddFluentCaptcha(
        this IServiceCollection services,
        Action<IFluentCaptchaConfigurator>? configure = null)
    {
        var configurator = new FluentCaptchaConfigurator(services);

        configure?.Invoke(configurator);

        services.Configure<FluentCaptchaOptions>(options =>
        {
            options.DefaultCaptchaProvider = configurator.DefaultCaptchaProvider;
            options.DefaultCaptchaResponseTokenSource = configurator.DefaultCaptchaResponseTokenSource;
        });

        return services;
    }
}