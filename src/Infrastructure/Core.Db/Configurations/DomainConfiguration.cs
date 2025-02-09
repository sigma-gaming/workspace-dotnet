using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class DomainConfiguration : IEntityTypeConfiguration<DomainEntity>
{
    public void Configure(EntityTypeBuilder<DomainEntity> entity)
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