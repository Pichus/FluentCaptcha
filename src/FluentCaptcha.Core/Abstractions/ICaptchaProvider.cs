namespace FluentCaptcha.Core.Abstractions;

public interface ICaptchaProvider : ICaptchaValidator
{
    static virtual string Name => throw new NotImplementedException();
}