using FluentCaptcha.Core;

namespace FluentCaptcha.Dummy;

public static class FluentCaptchaDummyConfiguratorExtensions
{
    public static FluentCaptchaConfigurator UseDummy(
        this FluentCaptchaConfigurator configurator)
    {
        configurator.AddDummy();

        configurator.UseCaptchaValidator<DummyCaptchaValidator>();
        configurator.UseCaptchaProvider<DummyCaptchaProvider>();

        return configurator;
    }

    public static FluentCaptchaConfigurator AddDummy(
        this FluentCaptchaConfigurator configurator)
    {
        configurator.AddCaptchaValidator<DummyCaptchaValidator>();
        configurator.AddCaptchaProvider<DummyCaptchaProvider>();

        return configurator;
    }
}