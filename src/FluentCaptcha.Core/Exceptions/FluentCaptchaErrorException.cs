namespace FluentCaptcha.Core.Exceptions;

public class FluentCaptchaErrorException : Exception
{
    public FluentCaptchaErrorException()
    {
    }

    public FluentCaptchaErrorException(string message)
        : base(message)
    {
    }

    public FluentCaptchaErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}