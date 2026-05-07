using FluentCaptcha.Core;
using FluentCaptcha.Examples.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FluentCaptcha.Examples.WebApi.Controllers;

[ApiController]
[Route("contact-form")]
public class ContactFormController : ControllerBase
{
    [HttpPost]
    [ValidateCaptcha]
    [ProducesResponseType<ContactFormSubmission>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitContactFormAsync([FromBody] ContactFormSubmission contactFormSubmission)
    {
        return Ok(contactFormSubmission);
    }
}