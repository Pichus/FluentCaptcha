namespace FluentCaptcha.Core.Attributes;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter)]
public class CaptchaResponseTokenAttribute : Attribute
{
}