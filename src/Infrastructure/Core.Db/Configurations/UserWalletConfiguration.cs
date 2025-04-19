using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class UserWalletConfiguration : IEntityTypeConfiguration<UserWalletEntity>
{
    public void Configure(EntityTypeBuilder<UserWalletEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("user_wallet");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.Provider)
            .HasColumnType("smallint")
            .HasColumnName("provider");
        entity.Property(e => e.Currency)
            .HasColumnType("smallint")
            .HasColumnName("currency");
        entity.Property(e => e.WalletAddress).HasColumnName("wallet_address");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("updated_at");
        entity.Property(e => e.IsActive)
            .HasDefaultValue(true)
            .HasColumnName("is_active");

        // Add a unique constraint for user_id, provider, currency combination
        entity.HasIndex(e => new { e.UserId, e.Provider, e.Currency })
            .IsUnique()
            .HasFilter("is_active = true");

        entity.HasOne(d => d.User).WithMany(p => p.UserWallets)
            .HasForeignKey(d => d.UserId);
    }
}
