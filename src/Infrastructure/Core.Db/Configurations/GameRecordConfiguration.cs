using Common.Db.Extensions;
using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class GameRecordConfiguration : IEntityTypeConfiguration<GameRecordEntity>
{
    public void Configure(EntityTypeBuilder<GameRecordEntity> entity)
    {
        entity.HasKey(e => e.Id);

        entity.ToTable("game_record");

        entity.HasIndex(e => e.UserId);

        entity.Property(e => e.Id)
            .HasDefaultValueSql("uuid_generate_v7()")
            .HasColumnName("id");
        entity.Property(e => e.Bet).HasColumnName("bet");
        entity.Property(e => e.CreatedAt)
            .HasDefaultValueSql("now()")
            .HasColumnName("created_at");
        entity.Property(e => e.Game)
            .HasColumnType("smallint")
            .HasColumnName("game");
        entity.Property(e => e.Multiplier).HasColumnName("multiplier");
        entity.Property(e => e.Outcome)
            .HasColumnType("smallint")
            .HasColumnName("outcome");
        entity.Property(e => e.Payout).HasColumnName("payout");
        entity.Property(e => e.PreviewUserName).HasColumnName("preview_user_name");
        entity.Property(e => e.Snapshot)
            .HasColumnType("jsonb")
            .HasColumnName("snapshot")
            .HasJsonbConversion();
        entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
        entity.Property(e => e.UserId).HasColumnName("user_id");

        entity.HasOne(d => d.Transaction).WithMany(p => p.GameRecords)
            .HasForeignKey(d => d.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.User).WithMany(p => p.GameRecords)
            .HasForeignKey(d => d.UserId);
    }
}