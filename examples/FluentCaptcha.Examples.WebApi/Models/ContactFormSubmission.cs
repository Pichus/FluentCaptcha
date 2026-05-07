using System.ComponentModel.DataAnnotations;

namespace FluentCaptcha.Examples.WebApi.Models;

public class ContactFormSubmission
{
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public required string Name { get; init; }
    
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string Email { get; init; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(1000)]
    public required string Message { get; init; }
}