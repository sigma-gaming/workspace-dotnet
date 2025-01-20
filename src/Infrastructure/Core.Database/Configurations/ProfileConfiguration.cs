using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("profile");

        entity.HasIndex(e => e.UserId).IsUnique();
        entity.HasIndex(e => e.Username).IsUnique();

        entity.Property(e => e.Id)
            .UseIdentityAlwaysColumn()
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.HasCustomName)
            .HasDefaultValue(false)
            .HasColumnName("has_custom_name");
        entity.Property(e => e.Image).HasColumnName("image");
        entity.Property(e => e.Name).HasColumnName("name");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("updated_at");
        entity.Property(e => e.UserId).HasColumnName("user_id");
        entity.Property(e => e.Username).HasColumnName("username");
        entity.Property(e => e.UsedProvider)
            .HasColumnType("smallint")
            .HasColumnName("used_provider");

        entity.HasOne(d => d.User).WithOne(p => p.Profile)
            .HasForeignKey<Profile>(d => d.UserId);
    }
}