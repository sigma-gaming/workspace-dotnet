using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class ReferrerPayoutConfiguration : IEntityTypeConfiguration<ReferrerPayoutEntity>
{
    public void Configure(EntityTypeBuilder<ReferrerPayoutEntity> entity)
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
            .HasForeignKey<ReferrerPayoutEntity>(d => d.ReferrerId);
    }
}