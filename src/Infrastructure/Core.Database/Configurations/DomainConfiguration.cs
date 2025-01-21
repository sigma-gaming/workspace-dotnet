using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class DomainConfiguration : IEntityTypeConfiguration<Contracts.Entities.DomainEntity>
{
    public void Configure(EntityTypeBuilder<Contracts.Entities.DomainEntity> entity)
    {
        entity.HasKey(e => e.Host);

        entity.ToTable("domain");

        entity.Property(e => e.App)
            .HasColumnType("smallint")
            .HasColumnName("app");
        entity.Property(e => e.Host).HasColumnName("host");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
    }
} 