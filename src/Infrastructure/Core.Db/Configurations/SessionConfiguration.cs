using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
{
    public void Configure(EntityTypeBuilder<SessionEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("session");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
        entity.Property(e => e.Provider)
            .HasColumnType("smallint")
            .HasColumnName("provider");
        entity.Property(e => e.ReferralCampaignId).HasColumnName("referral_campaign_id");
        entity.Property(e => e.ReferrerId).HasColumnName("referrer_id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.Roles).HasColumnName("roles");
        entity.Property(e => e.Roles)
            .HasColumnType("smallint[]")
            .HasColumnName("roles");

        entity.HasOne(d => d.ReferralCampaign).WithMany(p => p.Sessions)
            .HasForeignKey(d => d.ReferralCampaignId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(d => d.Referrer).WithMany(p => p.SessionReferrers)
            .HasForeignKey(d => d.ReferrerId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(d => d.User).WithMany(p => p.SessionUsers)
            .HasForeignKey(d => d.UserId);
    }
}