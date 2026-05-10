using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Enums;

namespace FluentCaptcha.Core;

public interface IFluentCaptchaConfigurator
{
    void AddCaptchaProvider<TCaptchaProvider>(string captchaProviderName, bool asTypedHttpClient = false)
        where TCaptchaProvider : class, ICaptchaValidator;

    void AddOptions<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class;
    
    string? DefaultCaptchaProvider { get; set; }

    CaptchaResponseTokenSource DefaultCaptchaResponseTokenSource { get; set; }
}