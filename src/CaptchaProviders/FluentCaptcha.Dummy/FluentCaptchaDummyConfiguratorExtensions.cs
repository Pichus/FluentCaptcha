using FluentCaptcha.Core;

namespace FluentCaptcha.Dummy;

public static class FluentCaptchaDummyConfiguratorExtensions
{
    public static FluentCaptchaConfigurator UseDummy(
        this FluentCaptchaConfigurator configurator)
    {
        configurator.AddDummy();
        configurator.DefaultCaptchaProvider = DummyConstants.CaptchaProviderName;

        return configurator;
    }

    public static FluentCaptchaConfigurator AddDummy(
        this FluentCaptchaConfigurator configurator)
    {
        configurator.AddCaptchaProvider<DummyCaptchaValidator>(DummyConstants.CaptchaProviderName);
        return configurator;
    }
}