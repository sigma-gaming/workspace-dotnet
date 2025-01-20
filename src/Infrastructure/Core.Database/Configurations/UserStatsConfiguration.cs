using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class UserStatsConfiguration : IEntityTypeConfiguration<UserStats>
{
    public void Configure(EntityTypeBuilder<UserStats> entity)
    {
        entity.HasKey(e => e.UserId);

        entity.ToTable("user_stats");

        entity.Property(e => e.UserId)
            .ValueGeneratedNever()
            .HasColumnName("user_id");
        entity.Property(e => e.DepositCount)
            .HasDefaultValue(0)
            .HasColumnName("deposit_count");
        entity.Property(e => e.WithdrawCount)
            .HasDefaultValue(0)
            .HasColumnName("withdraw_count");

        entity.HasOne(d => d.User).WithOne(p => p.UserStats)
            .HasForeignKey<UserStats>(d => d.UserId);
    }
} 