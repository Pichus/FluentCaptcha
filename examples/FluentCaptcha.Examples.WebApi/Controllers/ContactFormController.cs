using FluentCaptcha.Core.Attributes;
using FluentCaptcha.Dummy;
using FluentCaptcha.Examples.WebApi.Models;
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

    [HttpPost("cf")]
    [ValidateCaptcha]
    [ProducesResponseType<ContactFormSubmission>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitContactFormAsyncCf(
        [FromBody] ContactFormSubmission contactFormSubmission)
    {
        return Ok(contactFormSubmission);
    }

    [HttpPost("dummy")]
    [ValidateCaptcha(CaptchaProvider = DummyConstants.CaptchaProviderName)]
    [ProducesResponseType<ContactFormSubmission>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitContactFormAsyncDummy(
        [FromBody] ContactFormSubmission contactFormSubmission)
    {
        return Ok(contactFormSubmission);
    }
}