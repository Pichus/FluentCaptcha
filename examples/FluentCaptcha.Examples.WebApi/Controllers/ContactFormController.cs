using FluentCaptcha.Core.Attributes;
using FluentCaptcha.Core.Enums;
using FluentCaptcha.Dummy;
using FluentCaptcha.Examples.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentCaptcha.Examples.WebApi.Controllers;

[ApiController]
[Route("contact-form")]
public class ContactFormController : ControllerBase
{
    private readonly ILogger<ContactFormController> _logger;

    public ContactFormController(ILogger<ContactFormController> logger)
    {
        _logger = logger;
    }

    [HttpPost("cf-body")]
    [ValidateCaptcha(CaptchaResponseTokenSource = CaptchaResponseTokenSource.RequestBody)]
    public async Task<IActionResult> SubmitContactFormAsyncCfBody(
        [FromBody] TestModel testModel)
    {
        return Ok(testModel.Name);
    }

    [HttpPost("cf")]
    [ValidateCaptcha(
        CaptchaResponseTokenRequestHeaderName = "cf-token",
        ExpectedAction = "dfs")]
    [ProducesResponseType<ContactFormSubmission>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitContactFormAsyncCf(
        [FromBody] ContactFormSubmission contactFormSubmission)
    {
        return Ok(contactFormSubmission);
    }

    [HttpPost("dummy")]
    [ValidateCaptcha(
        CaptchaProvider = DummyConstants.CaptchaProviderName,
        CaptchaResponseTokenSource = CaptchaResponseTokenSource.RequestBody
    )]
    [ProducesResponseType<ContactFormSubmission>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitContactFormAsyncDummy(
        [FromBody] ContactFormSubmission contactFormSubmission)
    {
        return Ok(contactFormSubmission);
    }
}