using Microsoft.EntityFrameworkCore;
using Notification.Shared.Entities;
using Notification.Shared.Enums;

namespace Notification.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<OutboundMessage> OutboundMessages => Set<OutboundMessage>();

    public DbSet<MessageDeliveryEvent> MessageDeliveryEvents => Set<MessageDeliveryEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboundMessage>(entity =>
        {
            entity.Property(x => x.VendorReference).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Recipient).HasMaxLength(320).IsRequired();
            entity.Property(x => x.Subject).HasMaxLength(200);
            entity.Property(x => x.Body).IsRequired();
            entity.Property(x => x.ProviderName).HasMaxLength(100);
            entity.Property(x => x.ProviderMessageId).HasMaxLength(200);
            entity.Property(x => x.FailureReason).HasMaxLength(2000);
            entity.Property(x => x.SenderId).HasMaxLength(50);
            entity.Property(x => x.Channel).HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.RawPayload).HasColumnType("jsonb");
            entity.HasIndex(x => new { x.Channel, x.VendorReference }).IsUnique();
            entity.HasIndex(x => x.ProviderMessageId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.CreatedAt);
            entity.HasMany(x => x.Events)
                .WithOne(x => x.OutboundMessage)
                .HasForeignKey(x => x.OutboundMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MessageDeliveryEvent>(entity =>
        {
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(x => x.Detail).HasMaxLength(2000);
            entity.HasIndex(x => x.OutboundMessageId);
        });
    }
}
