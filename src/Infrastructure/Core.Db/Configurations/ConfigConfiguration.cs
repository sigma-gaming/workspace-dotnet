using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class ConfigConfiguration : IEntityTypeConfiguration<ConfigEntity>
{
    public void Configure(EntityTypeBuilder<ConfigEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("config");

        entity.Property(e => e.Id)
            .HasColumnName("id");
        entity.Property(e => e.BackgroundJobsEnabled)
            .HasDefaultValue(true)
            .HasColumnName("background_jobs_enabled");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.MaintenanceEnabled)
            .HasDefaultValue(false)
            .HasColumnName("maintenance_enabled");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("updated_at");
    }
}