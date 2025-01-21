using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class WithdrawalConfiguration : IEntityTypeConfiguration<WithdrawalEntity>
{
    public void Configure(EntityTypeBuilder<WithdrawalEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("withdrawal");

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.Currency)
            .HasColumnType("smallint")
            .HasColumnName("currency");
        entity.Property(e => e.CurrencyAmount)
            .HasPrecision(20, 18)
            .HasColumnName("currency_amount");
        entity.Property(e => e.GemAmount).HasColumnName("gem_amount");
        entity.Property(e => e.Method)
            .HasColumnType("smallint")
            .HasColumnName("method");
        entity.Property(e => e.ProviderAmount).HasColumnName("provider_amount");
        entity.Property(e => e.ProviderTransactionId).HasColumnName("provider_transaction_id");
        entity.Property(e => e.Status)
            .HasColumnType("smallint")
            .HasColumnName("status");
        entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
        entity.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("updated_at");
        entity.Property(e => e.Provider)
            .HasColumnType("smallint")
            .HasColumnName("provider");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.Transaction).WithMany(p => p.Withdrawals)
            .HasForeignKey(d => d.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.User).WithMany(p => p.Withdrawals)
            .HasForeignKey(d => d.UserId);
    }
} 