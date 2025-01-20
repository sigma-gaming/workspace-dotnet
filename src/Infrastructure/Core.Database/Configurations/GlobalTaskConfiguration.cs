using Core.Domain.Entities;
using Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class GlobalTaskConfiguration : IEntityTypeConfiguration<GlobalTask>
{
    public void Configure(EntityTypeBuilder<GlobalTask> entity)
    {
        entity.HasNoKey();
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
            .HasColumnName("requirements");
        entity.Property(e => e.WageringMultiplier).HasColumnName("wagering_multiplier");
    }
} 