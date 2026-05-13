using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace FluentCaptcha.Core;

public class FluentCaptchaConfigurator : IFluentCaptchaConfigurator
{
    private readonly IServiceCollection _services;

    public FluentCaptchaConfigurator(IServiceCollection services)
    {
        _services = services;
    }

    public string? DefaultCaptchaProvider { get; set; }

    public CaptchaResponseTokenSource DefaultCaptchaResponseTokenSource { get; set; } =
        CaptchaResponseTokenSource.RequestHeader;

    public string? DefaultCaptchaResponseTokenRequestHeaderName { get; set; }

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
}