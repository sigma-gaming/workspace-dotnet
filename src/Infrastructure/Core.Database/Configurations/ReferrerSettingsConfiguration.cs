using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class ReferrerSettingsConfiguration : IEntityTypeConfiguration<ReferrerSettings>
{
  public void Configure(EntityTypeBuilder<ReferrerSettings> entity)
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
        .HasForeignKey<ReferrerSettings>(d => d.ReferrerId);
  }
}