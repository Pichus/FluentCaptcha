using FluentCaptcha.Core.Abstractions;
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

        return services;
    }
}