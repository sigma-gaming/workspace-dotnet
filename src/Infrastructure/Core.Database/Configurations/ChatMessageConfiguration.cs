using Core.Domain.Entities;
using Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("chat_message");

        entity.HasIndex(e => e.IsPinned).HasFilter("(\"is_pinned\" = true)");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.Attachments)
            .HasDefaultValueSql("'[]'::jsonb")
            .HasColumnType("jsonb")
            .HasColumnName("attachments");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.IsPinned)
            .HasDefaultValue(false)
            .HasColumnName("is_pinned");
        entity.Property(e => e.ProfileId).HasColumnName("profile_id");
        entity.Property(e => e.SenderImage).HasColumnName("sender_image");
        entity.Property(e => e.SenderName).HasColumnName("sender_name");
        entity.Property(e => e.SenderUsername).HasColumnName("sender_username");
        entity.Property(e => e.SenderRoles)
            .HasColumnType("smallint[]")
            .HasColumnName("sender_roles");
        entity.Property(e => e.Text).HasColumnName("text");
        entity.Property(e => e.TrackingId)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("tracking_id");
        entity.Property(e => e.Type)
            .HasColumnType("smallint")
            .HasColumnName("type");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.Profile).WithMany(p => p.ChatMessages)
            .HasForeignKey(d => d.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.User).WithMany(p => p.ChatMessages)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}