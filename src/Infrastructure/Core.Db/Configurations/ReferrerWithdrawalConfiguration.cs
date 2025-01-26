using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class ReferrerWithdrawalConfiguration : IEntityTypeConfiguration<ReferrerWithdrawalEntity>
{
    public void Configure(EntityTypeBuilder<ReferrerWithdrawalEntity> entity)
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