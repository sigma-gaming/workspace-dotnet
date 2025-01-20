using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class ReferrerPayoutConfiguration : IEntityTypeConfiguration<ReferrerPayout>
{
    public void Configure(EntityTypeBuilder<ReferrerPayout> entity)
    {
        entity.HasKey(e => e.ReferrerId);

        entity.ToTable("referrer_payout");

        entity.Property(e => e.ReferrerId)
            .ValueGeneratedNever()
            .HasColumnName("referrer_id");
        entity.Property(e => e.LastPayoutAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_payout_at");
        entity.Property(e => e.NextPayoutAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("next_payout_at");

        entity.HasOne(d => d.Referrer).WithOne(p => p.ReferrerPayout)
            .HasForeignKey<ReferrerPayout>(d => d.ReferrerId);
    }
} 