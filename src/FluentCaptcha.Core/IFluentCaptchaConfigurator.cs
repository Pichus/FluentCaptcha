using FluentCaptcha.Core.Abstractions;

namespace FluentCaptcha.Core;

public interface IFluentCaptchaConfigurator
{
    void AddCaptchaProvider<TCaptchaProvider>(string captchaProviderName, bool asTypedHttpClient = false)
        where TCaptchaProvider : class, ICaptchaValidator;

    void AddOptions<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class;

    void SetDefaultCaptchaProvider(string captchaProvider);
}