namespace FluentCaptcha.Core.Exceptions;

public class FluentCaptchaNetworkErrorException : FluentCaptchaErrorException
{
    public FluentCaptchaNetworkErrorException()
    {
    }

    public FluentCaptchaNetworkErrorException(string message)
        : base(message)
    {
    }

    public FluentCaptchaNetworkErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}