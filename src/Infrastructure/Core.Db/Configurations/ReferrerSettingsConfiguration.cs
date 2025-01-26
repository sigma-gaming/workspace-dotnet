using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class ReferrerSettingsConfiguration : IEntityTypeConfiguration<ReferrerSettingsEntity>
{
  public void Configure(EntityTypeBuilder<ReferrerSettingsEntity> entity)
  {
    entity.HasKey(e => e.ReferrerId);

    entity.ToTable("referrer_settings");

    entity.Property(e => e.ReferrerId)
        .ValueGeneratedNever()
        .HasColumnName("referrer_id");
    entity.Property(e => e.RevShare)
        .HasDefaultValue(10)
        .HasColumnName("rev_share");

    entity.HasOne(d => d.Referrer).WithOne(p => p.ReferrerSettings)
        .HasForeignKey<ReferrerSettingsEntity>(d => d.ReferrerId);
  }
}