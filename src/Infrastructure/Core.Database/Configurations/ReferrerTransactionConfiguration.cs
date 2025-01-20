using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class ReferrerTransactionConfiguration : IEntityTypeConfiguration<ReferrerTransaction>
{
    public void Configure(EntityTypeBuilder<ReferrerTransaction> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("referrer_transaction");

        entity.HasIndex(e => e.CreatedAt);
        entity.HasIndex(e => e.ReferrerId);

        entity.Property(e => e.ReferralAction)
            .HasColumnType("smallint")
            .HasColumnName("referral_action");
        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.Amount).HasColumnName("amount");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.IsProcessed)
            .HasDefaultValue(false)
            .HasColumnName("is_processed");
        entity.Property(e => e.ReferralCampaignId).HasColumnName("referral_campaign_id");
        entity.Property(e => e.ReferralId).HasColumnName("referral_id");
        entity.Property(e => e.ReferrerId).HasColumnName("referrer_id");

        entity.HasOne(d => d.ReferralCampaign).WithMany(p => p.ReferrerTransactions)
            .HasForeignKey(d => d.ReferralCampaignId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(d => d.Referral).WithMany(p => p.ReferrerTransactionReferrals)
            .HasForeignKey(d => d.ReferralId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(d => d.Referrer).WithMany(p => p.ReferrerTransactionReferrers)
            .HasForeignKey(d => d.ReferrerId);
    }
} 