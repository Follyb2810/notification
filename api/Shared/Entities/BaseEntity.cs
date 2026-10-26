using System.ComponentModel.DataAnnotations;

namespace Notification.Shared.Entities;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
