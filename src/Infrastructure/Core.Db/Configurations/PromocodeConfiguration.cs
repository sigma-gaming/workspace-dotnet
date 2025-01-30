using Core.Contracts.Entities;
using Common.Db.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class PromocodeConfiguration : IEntityTypeConfiguration<PromocodeEntity>
{
    public void Configure(EntityTypeBuilder<PromocodeEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("promocode");

        entity.HasIndex(e => e.Code).IsUnique();
        entity.HasIndex(e => e.ExpiresAt);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.Bonus)
            .HasColumnType("jsonb")
            .HasColumnName("bonus")
            .HasJsonbConversion();
        entity.Property(e => e.BonusType)
            .HasColumnType("smallint")
            .HasColumnName("bonus_type");
        entity.Property(e => e.Campaign).HasColumnName("campaign");
        entity.Property(e => e.Code).HasColumnName("code");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.CreatedBy).HasColumnName("created_by");
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
        entity.Property(e => e.IsActive).HasColumnName("is_active");
        entity.Property(e => e.MaxUsages).HasColumnName("max_usages");
        entity.Property(e => e.Usages).HasColumnName("usages");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.WageringMultiplier).HasColumnName("wagering_multiplier");

        entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PromocodeCreatedByNavigations)
            .HasForeignKey(d => d.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(d => d.User).WithMany(p => p.PromocodeUsers)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}