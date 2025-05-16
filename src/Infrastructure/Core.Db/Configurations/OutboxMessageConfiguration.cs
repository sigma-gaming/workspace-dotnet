using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessageEntity>
{
    public void Configure(EntityTypeBuilder<OutboxMessageEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("outbox_message");

        entity.HasIndex(e => e.CreatedAt).HasFilter("(\"published_at\" IS NULL)");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.PublishedAt).HasColumnName("published_at");
        entity.Property(e => e.Exchange).HasColumnName("exchange");
        entity.Property(e => e.RoutingKey).HasColumnName("routing_key");
        entity.Property(e => e.Payload)
            .HasColumnType("jsonb")
            .HasColumnName("payload");
    }
}