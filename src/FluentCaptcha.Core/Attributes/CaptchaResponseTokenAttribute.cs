namespace FluentCaptcha.Core.Attributes;

/// <summary>
///     Indicates that property is used to store captcha response token.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class CaptchaResponseTokenAttribute : Attribute
{
}