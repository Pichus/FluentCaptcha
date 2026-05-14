using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Core.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel;

namespace FluentCaptcha.Core;

public class FluentCaptchaConfigurator
{
    public FluentCaptchaConfigurator(IServiceCollection services)
    {
        Services = services;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IServiceCollection Services { get; }

    public string? DefaultCaptchaProvider { get; set; }

    public CaptchaResponseTokenSource DefaultCaptchaResponseTokenSource { get; set; } =
        CaptchaResponseTokenSource.RequestHeader;

    public string DefaultCaptchaResponseTokenRequestHeaderName { get; set; } =
        FluentCaptchaConstants.DefaultCaptchaResponseTokenRequestHeaderName;

    public void AddCaptchaProvider<TCaptchaProvider>()
        where TCaptchaProvider : class, ICaptchaProvider
    {
        Services.TryAddKeyedScoped<ICaptchaProvider, TCaptchaProvider>(
            GetCaptchaProviderServiceKey<TCaptchaProvider>());
    }

    public void UseCaptchaProvider<TCaptchaProvider>()
        where TCaptchaProvider : class, ICaptchaProvider
    {
        Services.TryAddScoped<ICaptchaProvider>(serviceProvider =>
            serviceProvider.GetRequiredKeyedService<ICaptchaProvider>(
                GetCaptchaProviderServiceKey<TCaptchaProvider>()));
        DefaultCaptchaProvider = TCaptchaProvider.Name;
    }

    public void AddCaptchaValidator<TCaptchaValidator>()
        where TCaptchaValidator : class, ICaptchaValidator
    {
        Services.TryAddScoped<TCaptchaValidator>();
    }

    public void UseCaptchaValidator<TCaptchaValidator>()
        where TCaptchaValidator : class, ICaptchaValidator
    {
        Services.TryAddScoped<ICaptchaValidator>(serviceProvider =>
            serviceProvider.GetRequiredService<TCaptchaValidator>());
    }

    public FluentCaptchaConfigurator Configure(Action<FluentCaptchaOptions> options)
    {
        Services.Configure(options);
        return this;
    }

    private static string GetCaptchaProviderServiceKey<TCaptchaProvider>()
        where TCaptchaProvider : class, ICaptchaProvider
    {
        return FluentCaptchaConstants.LibraryPrefix + TCaptchaProvider.Name;
    }
}