using Core.Domain.Entities;
using Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class ReferrerWithdrawalConfiguration : IEntityTypeConfiguration<ReferrerWithdrawal>
{
    public void Configure(EntityTypeBuilder<ReferrerWithdrawal> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("referrer_withdrawal");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.Amount).HasColumnName("amount");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.ReferrerId).HasColumnName("referrer_id");

        entity.HasOne(d => d.Referrer).WithMany(p => p.ReferrerWithdrawals)
            .HasForeignKey(d => d.ReferrerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 