using FluentCaptcha.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCaptcha.Core;

public class FluentCaptchaConfigurator : IFluentCaptchaConfigurator
{
    private readonly IServiceCollection _services;

    public FluentCaptchaConfigurator(IServiceCollection services)
    {
        _services = services;
    }

    public string? DefaultCaptchaProvider { get; private set; }

    public void AddCaptchaProvider<TCaptchaProvider>(string captchaProviderName, bool asTypedHttpClient = false)
        where TCaptchaProvider : class, ICaptchaValidator
    {
        var serviceKey = FluentCaptchaConstants.LibraryPrefix + captchaProviderName;

        if (asTypedHttpClient)
        {
            _services.AddHttpClient<ICaptchaValidator, TCaptchaProvider>(serviceKey).AddAsKeyed();
        }

        _services.AddKeyedScoped<ICaptchaValidator, TCaptchaProvider>(serviceKey);
    }

    public void AddOptions<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class
    {
        _services.Configure(configureOptions);
    }

    public void SetDefaultCaptchaProvider(string captchaProvider)
    {
        DefaultCaptchaProvider = captchaProvider;
    }
}