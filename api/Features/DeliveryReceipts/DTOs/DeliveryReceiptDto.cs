using System.ComponentModel.DataAnnotations;
using Notification.Shared.Enums;

namespace Notification.Features.DeliveryReceipts.DTOs;

public class DeliveryReceiptDto
{
    [MaxLength(200)]
    public string? ProviderMessageId { get; set; }

    [MaxLength(100)]
    public string? VendorReference { get; set; }

    public MessageChannel? Channel { get; set; }

    public bool Delivered { get; set; }

    [MaxLength(2000)]
    public string? Detail { get; set; }
}
