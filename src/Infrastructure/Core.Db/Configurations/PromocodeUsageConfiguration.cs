using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class PromocodeUsageConfiguration : IEntityTypeConfiguration<PromocodeUsageEntity>
{
    public void Configure(EntityTypeBuilder<PromocodeUsageEntity> entity)
    {
        entity.HasKey(e => new { e.PromocodeId, e.UserId });

        entity.ToTable("promocode_usage");

        entity.Property(e => e.PromocodeId).HasColumnName("promocode_id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
        entity.Property(e => e.Status)
            .HasColumnType("smallint")
            .HasColumnName("status");

        entity.HasOne(d => d.Promocode).WithMany(p => p.PromocodeUsages)
            .HasForeignKey(d => d.PromocodeId);

        entity.HasOne(d => d.User).WithMany(p => p.PromocodeUsages)
            .HasForeignKey(d => d.UserId);
    }
} 