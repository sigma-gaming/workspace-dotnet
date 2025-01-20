using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class DomainConfiguration : IEntityTypeConfiguration<Models.Entities.Domain>
{
    public void Configure(EntityTypeBuilder<Models.Entities.Domain> entity)
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