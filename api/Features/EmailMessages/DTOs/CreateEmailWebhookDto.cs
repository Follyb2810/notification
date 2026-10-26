using System.ComponentModel.DataAnnotations;

namespace Notification.Features.EmailMessages.DTOs;

public class CreateEmailWebhookDto
{
    [Required]
    [MaxLength(100)]
    public string Reference { get; set; } = string.Empty;

    [Required]
    [MaxLength(320)]
    [EmailAddress]
    public string To { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    [MaxLength(20000)]
    public string Body { get; set; } = string.Empty;

    public bool IsHtml { get; set; }
}
