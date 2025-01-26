using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("transaction");

        entity.HasIndex(e => e.CreatedAt);
        entity.HasIndex(e => e.UserId);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.Amount).HasColumnName("amount");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.Game)
            .HasColumnType("smallint")
            .HasColumnName("game");
        entity.Property(e => e.GameRecordId).HasColumnName("game_record_id");
        entity.Property(e => e.Type)
            .HasColumnType("smallint")
            .HasColumnName("type");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.User).WithMany(p => p.Transactions)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
} 