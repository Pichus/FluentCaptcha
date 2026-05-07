namespace FluentCaptcha.Core.Exceptions;

public class FluentCaptchaConfigurationException : FluentCaptchaErrorException
{
    public FluentCaptchaConfigurationException()
    {
    }

    public FluentCaptchaConfigurationException(string message)
        : base(message)
    {
    }

    public FluentCaptchaConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}