using FluentCaptcha.Core.Attributes;

namespace FluentCaptcha.Examples.WebApi;

public class TestModel
{
    public string Name { get; init; }

    [CaptchaResponseToken]
    public string ResponseToken { get; init; }
}