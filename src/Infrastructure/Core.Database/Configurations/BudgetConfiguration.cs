using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("budget");

        entity.Property(e => e.Id)
            .HasColumnName("id");
        entity.Property(e => e.Available)
            .HasDefaultValue(10000000L)
            .HasColumnName("available");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.LastSyncAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("last_sync_at");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("updated_at");
    }
}