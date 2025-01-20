using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("account");

        entity.HasIndex(e => e.ProviderUserId);

        entity.HasIndex(e => e.UserId);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.Provider)
            .HasColumnType("smallint")
            .HasColumnName("provider");
        entity.Property(e => e.ProviderUserFirstName).HasColumnName("provider_user_first_name");
        entity.Property(e => e.ProviderUserId).HasColumnName("provider_user_id");
        entity.Property(e => e.ProviderUserImage).HasColumnName("provider_user_image");
        entity.Property(e => e.ProviderUserLastName).HasColumnName("provider_user_last_name");
        entity.Property(e => e.ProviderUsername).HasColumnName("provider_username");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("updated_at");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User).WithMany(p => p.Accounts)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 