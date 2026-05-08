using FluentCaptcha.Core;
using FluentCaptcha.Dummy;
using FluentCaptcha.Examples.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentCaptcha.Examples.WebApi.Controllers;

[ApiController]
[Route("contact-form")]
public class ContactFormController : ControllerBase
{
    [HttpPost("cf")]
    [ValidateCaptcha]
    [ProducesResponseType<ContactFormSubmission>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitContactFormAsyncCf([FromBody] ContactFormSubmission contactFormSubmission)
    {
        return Ok(contactFormSubmission);
    }

    [HttpPost("dummy")]
    [ValidateCaptcha(CaptchaProvider = DummyConstants.CaptchaProviderName)]
    [ProducesResponseType<ContactFormSubmission>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitContactFormAsyncDummy([FromBody] ContactFormSubmission contactFormSubmission)
    {
        return Ok(contactFormSubmission);
    }
}