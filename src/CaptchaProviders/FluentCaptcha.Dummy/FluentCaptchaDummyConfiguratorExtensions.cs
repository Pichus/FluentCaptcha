using FluentCaptcha.Core;

namespace FluentCaptcha.Dummy;

public static class FluentCaptchaDummyConfiguratorExtensions
{
    public static IFluentCaptchaConfigurator UseDummy(
        this IFluentCaptchaConfigurator configurator)
    {
        configurator.AddDummy();
        configurator.DefaultCaptchaProvider = DummyConstants.CaptchaProviderName;

        return configurator;
    }

    public static IFluentCaptchaConfigurator AddDummy(
        this IFluentCaptchaConfigurator configurator)
    {
        configurator.AddCaptchaProvider<DummyCaptchaValidator>(DummyConstants.CaptchaProviderName);
        return configurator;
    }
}