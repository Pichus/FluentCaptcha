using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
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

    public string? DefaultCaptchaResponseTokenRequestHeaderName { get; set; }

    public void AddCaptchaProvider<TCaptchaProvider>(string captchaProviderName, bool asTypedHttpClient = false)
        where TCaptchaProvider : class, ICaptchaValidator
    {
        var serviceKey = FluentCaptchaConstants.LibraryPrefix + captchaProviderName;

        if (asTypedHttpClient)
        {
            Services.AddHttpClient<ICaptchaValidator, TCaptchaProvider>(serviceKey).AddAsKeyed();
        }

        Services.AddKeyedScoped<ICaptchaValidator, TCaptchaProvider>(serviceKey);
    }

    public void AddOptions<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class
    {
        Services.Configure(configureOptions);
    }
}