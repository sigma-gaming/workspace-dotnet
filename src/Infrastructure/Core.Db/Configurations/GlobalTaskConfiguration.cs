using Common.Db.Extensions;
using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class GlobalTaskConfiguration : IEntityTypeConfiguration<GlobalTaskEntity>
{
    public void Configure(EntityTypeBuilder<GlobalTaskEntity> entity)
    {
        entity.HasKey(e => e.Key);
        entity.ToTable("global_task");

        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.IsActive)
            .HasDefaultValue(true)
            .HasColumnName("is_active");
        entity.Property(e => e.Key)
            .HasColumnType("smallint")
            .HasColumnName("key");
        entity.Property(e => e.Payout).HasColumnName("payout");
        entity.Property(e => e.Requirements)
            .HasColumnType("jsonb")
            .HasColumnName("requirements")
            .HasJsonbConversion();
        entity.Property(e => e.WageringMultiplier).HasColumnName("wagering_multiplier");
    }
}