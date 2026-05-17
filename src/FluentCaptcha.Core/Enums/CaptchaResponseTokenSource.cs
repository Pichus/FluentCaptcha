namespace FluentCaptcha.Core.Enums;

public enum CaptchaResponseTokenSource
{
    /// <summary>
    ///     Specifies that the library should use a default
    ///     <see cref="FluentCaptcha.Core.Enums.CaptchaResponseTokenSource">
    ///         <c>CaptchaResponseTokenSource</c>
    ///     </see>
    ///     value,
    ///     <see cref="FluentCaptcha.Core.Enums.CaptchaResponseTokenSource.RequestHeader">
    ///         <c>RequestHeader</c>
    ///     </see>
    ///     .
    /// </summary>
    Default,

    /// <summary>
    ///     Specifies that the captcha response token is stored somewhere in the request body.
    /// </summary>
    /// <remarks>
    ///     <see cref="FluentCaptcha.Core.Attributes.CaptchaResponseTokenAttribute">
    ///         <c>CaptchaResponseTokenAttribute</c>
    ///     </see>
    ///     must be applied to a property, located in any of the action arguments, containing the token for it
    ///     to be recognized by the library
    /// </remarks>
    RequestBody,

    /// <summary>
    ///     Specifies that the captcha response token is stored in the request header.
    /// </summary>
    /// <remarks>
    ///     By default, the library looks for X-Captcha-Response request header in the requests marked for captcha validation.
    ///     However, you may change it whether for a specific action by setting
    ///     <see cref="FluentCaptcha.Core.Attributes.ValidateCaptchaAttribute.CaptchaResponseTokenRequestHeaderName">
    ///         <c>CaptchaResponseTokenRequestHeaderName</c>
    ///     </see>
    ///     attribute parameter to whatever request header name you want or set a default setting for the whole app by setting
    ///     <see cref="FluentCaptcha.Core.FluentCaptchaConfigurator.DefaultCaptchaResponseTokenRequestHeaderName">
    ///         <c>options.DefaultCaptchaResponseTokenRequestHeaderName</c>
    ///     </see>
    ///     to a desired value during the fluent captcha setup (options inside
    ///     <see cref="FluentCaptchaServiceCollectionExtensions.AddFluentCaptcha">
    ///         <c>AddFluentCaptcha</c>
    ///     </see>
    ///     )
    /// </remarks>
    RequestHeader,

    /// <summary>
    ///     Specifies that the captcha response token is stored in the request form.
    /// </summary>
    /// <remarks>
    ///     By default, the library looks for X-Captcha-Response request header in the requests marked for captcha validation.
    ///     However, you may change it whether for a specific action by setting
    ///     <see cref="FluentCaptcha.Core.Attributes.ValidateCaptchaAttribute.CaptchaResponseTokenRequestHeaderName">
    ///         <c>CaptchaResponseTokenRequestHeaderName</c>
    ///     </see>
    ///     attribute parameter to whatever request header name you want or set a default setting for the whole app by setting
    ///     <see cref="FluentCaptcha.Core.FluentCaptchaConfigurator.DefaultCaptchaResponseTokenRequestHeaderName">
    ///         <c>options.DefaultCaptchaResponseTokenRequestHeaderName</c>
    ///     </see>
    ///     to a desired value during the fluent captcha setup (options inside
    ///     <see cref="FluentCaptchaServiceCollectionExtensions.AddFluentCaptcha">
    ///         <c>AddFluentCaptcha</c>
    ///     </see>
    ///     )
    /// </remarks>
    RequestForm
}