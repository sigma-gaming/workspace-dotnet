using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class ReferrerBalanceConfiguration : IEntityTypeConfiguration<ReferrerBalanceEntity>
{
    public void Configure(EntityTypeBuilder<ReferrerBalanceEntity> entity)
    {
        entity.HasKey(e => e.ReferrerId);

        entity.ToTable("referrer_balance");

        entity.Property(e => e.ReferrerId)
            .ValueGeneratedNever()
            .HasColumnName("referrer_id");
        entity.Property(e => e.Available)
            .HasDefaultValue(0L)
            .HasColumnName("available");

        entity.HasOne(d => d.Referrer).WithOne(p => p.ReferrerBalance)
            .HasForeignKey<ReferrerBalanceEntity>(d => d.ReferrerId);
    }
} 