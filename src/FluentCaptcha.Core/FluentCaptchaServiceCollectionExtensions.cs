using FluentCaptcha.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCaptcha.Core;

public static class FluentCaptchaServiceCollectionExtensions
{
    public static FluentCaptchaServicesBuilder AddFluentCaptcha(this IServiceCollection services)
    {
        var builder = new FluentCaptchaServicesBuilder(services);
        return builder;
    }

    public static FluentCaptchaServicesBuilder AddCloudflareCaptchaProvider(
        this FluentCaptchaServicesBuilder builder,
        Action<CloudflareTurnstileCaptchaOptions> configureCloudflareCaptcha)
    {
        builder.Services.AddHttpClient<ICaptchaValidator, CloudflareCaptchaValidator>();
        builder.Services.Configure<CloudflareTurnstileCaptchaOptions>(configureCloudflareCaptcha);
        return builder;
    }
}

public class FluentCaptchaServicesBuilder
{
    internal FluentCaptchaServicesBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}