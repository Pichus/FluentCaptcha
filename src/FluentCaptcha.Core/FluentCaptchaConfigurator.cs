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

    public void AddCaptchaProvider<TCaptchaProvider>()
        where TCaptchaProvider : class, ICaptchaValidator
    {
        _services.AddHttpClient<ICaptchaValidator, TCaptchaProvider>();
    }

    public void AddOptions<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class
    {
        _services.Configure(configureOptions);
    }
}