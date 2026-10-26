using System.ComponentModel.DataAnnotations;

namespace Notification.Features.TextMessages.DTOs;

public class CreateTextMessageWebhookDto
{
    [Required]
    [MaxLength(100)]
    public string Reference { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(1600)]
    public string Message { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? SenderId { get; set; }
}
