using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class ReferralCampaignConfiguration : IEntityTypeConfiguration<ReferralCampaignEntity>
{
    public void Configure(EntityTypeBuilder<ReferralCampaignEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("referral_campaign");

        entity.HasIndex(e => e.Code).IsUnique();

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.Code).HasColumnName("code");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.Name).HasColumnName("name");
        entity.Property(e => e.ReferrerId).HasColumnName("referrer_id");
        entity.Property(e => e.TotalSignups)
            .HasDefaultValue(0)
            .HasColumnName("total_signups");
        entity.Property(e => e.TotalVisits)
            .HasDefaultValue(0)
            .HasColumnName("total_visits");

        entity.HasOne(d => d.Referrer).WithMany(p => p.ReferralCampaigns)
            .HasForeignKey(d => d.ReferrerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 