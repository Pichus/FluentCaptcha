using FluentCaptcha.Core.Abstractions;

namespace FluentCaptcha.Core;

public interface IFluentCaptchaConfigurator
{
    void AddCaptchaProvider<TCaptchaProvider>()
        where TCaptchaProvider : class, ICaptchaValidator;

    void AddOptions<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class;
}