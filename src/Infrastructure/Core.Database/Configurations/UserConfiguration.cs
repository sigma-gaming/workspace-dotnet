using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Core.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("user");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.ProfileId).HasColumnName("profile_id");
        entity.Property(e => e.ReferralCampaignId).HasColumnName("referral_campaign_id");
        entity.Property(e => e.ReferrerId).HasColumnName("referrer_id");
        entity.Property(e => e.Roles)
            .HasColumnType("smallint[]")
            .HasColumnName("roles")
            .HasDefaultValueSql("'{0}'::smallint[]");
        entity.Property(e => e.SecurityId).HasColumnName("security_id");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("updated_at");
        entity.Property(e => e.Virtual)
            .HasDefaultValue(false)
            .HasColumnName("virtual");

        entity.HasOne(d => d.Referrer).WithMany(p => p.InverseReferrer)
            .HasForeignKey(d => d.ReferrerId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}