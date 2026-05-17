using FluentCaptcha.Core.Abstractions;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Core.Exceptions;
using FluentCaptcha.Core.Filters;
using FluentCaptcha.Core.Options;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FluentCaptcha.Core.Attributes;

/// <summary>
///     Specifies that the controller or the action method this attribute is applied to requires captcha validation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ValidateCaptchaAttribute : Attribute, IFilterFactory
{
    /// <summary>
    ///     Gets or sets the expected action name, so the actual action name specified by the client,
    ///     where the captcha is rendered, can be compared with it.
    /// </summary>
    public string? ExpectedAction { get; set; }

    /// <summary>
    ///     Gets or sets the captcha response token source.
    /// </summary>
    /// <remarks>
    ///     By default, the default captcha response token source, set during fluent captcha configuration, is used.
    /// </remarks>
    public CaptchaResponseTokenSource CaptchaResponseTokenSource { get; set; } = CaptchaResponseTokenSource.Default;

    /// <summary>
    ///     Gets or sets the captcha provider name to be used for validation of endpoints that reach this endpoint.
    /// </summary>
    /// <remarks>
    ///     It is strongly advised to pass predefined constants to this parameter.
    ///     <br />
    ///     E.g. <c>YourDesiredCaptchaProviderNameConstants.CaptchaProviderName</c>
    /// </remarks>
    public string? CaptchaProvider { get; set; }

    /// <summary>
    ///     Gets or sets the captcha response token request header name.
    /// </summary>
    /// <remarks>
    ///     Only takes effect when
    ///     <see cref="ValidateCaptchaAttribute.CaptchaResponseTokenSource">
    ///         <c>CaptchaResponseTokenSource</c>
    ///     </see>
    ///     is set to
    ///     <see cref="CaptchaResponseTokenSource.RequestHeader">
    ///         <c>RequestHeader</c>
    ///     </see>
    ///     .
    /// </remarks>
    public string? CaptchaResponseTokenRequestHeaderName { get; set; }

    /// <summary>
    ///     Gets or sets the captcha response token form parameter name;
    /// </summary>
    /// <remarks>
    ///     Only takes effect when
    ///     <see cref="ValidateCaptchaAttribute.CaptchaResponseTokenSource">
    ///         <c>CaptchaResponseTokenSource</c>
    ///     </see>
    ///     is set to
    ///     <see cref="CaptchaResponseTokenSource.RequestForm">
    ///         <c>RequestForm</c>
    ///     </see>
    ///     .
    ///     <br /> <br />
    ///     By default, the form parameter name provided by your selected
    ///     captcha provider (<see cref="ICaptchaProvider.CaptchaResponseTokenFormParameterName" />) is used.
    /// </remarks>
    public string? CaptchaResponseTokenFormParameterName { get; set; }

    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        var fluentCaptchaOptions = serviceProvider.GetService<IOptions<FluentCaptchaOptions>>()?.Value;

        if (fluentCaptchaOptions is null)
        {
            throw new FluentCaptchaConfigurationException("No config provided for fluent captcha.");
        }

        CaptchaResponseTokenSource = CaptchaResponseTokenSource == CaptchaResponseTokenSource.Default
            ? fluentCaptchaOptions.DefaultCaptchaResponseTokenSource
            : CaptchaResponseTokenSource;
        
        CaptchaResponseTokenRequestHeaderName ??= fluentCaptchaOptions.DefaultCaptchaResponseTokenRequestHeaderName;

        var captchaProviderName = fluentCaptchaOptions.DefaultCaptchaProvider;

        if (CaptchaProvider is not null)
        {
            captchaProviderName = CaptchaProvider;
        }

        var serviceKey = FluentCaptchaConstants.LibraryPrefix + captchaProviderName;
        var captchaProviderInstance = serviceProvider.GetKeyedService<ICaptchaProvider>(serviceKey);

        if (captchaProviderInstance is null)
        {
            throw new FluentCaptchaConfigurationException(
                $"No captcha providers registered with name '{captchaProviderName}'");
        }

        CaptchaResponseTokenFormParameterName ??= captchaProviderInstance.CaptchaResponseTokenFormParameterName;

        return new ValidateCaptchaActionFilter(
            captchaProviderInstance,
            CaptchaResponseTokenRequestHeaderName,
            CaptchaResponseTokenSource,
            CaptchaResponseTokenFormParameterName,
            ExpectedAction);
    }
}