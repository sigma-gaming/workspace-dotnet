using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("notification");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.AutoClose).HasColumnName("auto_close");
        entity.Property(e => e.AutoCloseMs).HasColumnName("auto_close_ms");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
        entity.Property(e => e.Kind)
            .HasColumnType("smallint")
            .HasColumnName("kind");
        entity.Property(e => e.Message).HasColumnName("message");
        entity.Property(e => e.Title).HasColumnName("title");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.WithCloseButton).HasColumnName("with_close_button");

        entity.HasOne(d => d.User).WithMany(p => p.Notifications)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 