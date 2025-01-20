using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Database.Configurations;

public class BalanceConfiguration : IEntityTypeConfiguration<Balance>
{
    public void Configure(EntityTypeBuilder<Balance> entity)
    {
        entity.HasKey(e => e.UserId);

        entity.ToTable("balance");

        entity.Property(e => e.UserId)
            .ValueGeneratedNever()
            .HasColumnName("user_id");
        entity.Property(e => e.Available)
            .HasDefaultValue(0L)
            .HasColumnName("available");
        entity.Property(e => e.MaxMultiplier)
            .HasDefaultValue(0)
            .HasColumnName("max_multiplier");
        entity.Property(e => e.MaxMultiplierGameId).HasColumnName("max_multiplier_game_id");
        entity.Property(e => e.MaxWin)
            .HasDefaultValue(0L)
            .HasColumnName("max_win");
        entity.Property(e => e.MaxWinGameId).HasColumnName("max_win_game_id");
        entity.Property(e => e.TotalBet)
            .HasDefaultValue(0L)
            .HasColumnName("total_bet");
        entity.Property(e => e.TotalBetCount)
            .HasDefaultValue(0)
            .HasColumnName("total_bet_count");
        entity.Property(e => e.TotalLost)
            .HasDefaultValue(0L)
            .HasColumnName("total_lost");
        entity.Property(e => e.TotalWin)
            .HasDefaultValue(0L)
            .HasColumnName("total_win");
        entity.Property(e => e.WageringRequired)
            .HasDefaultValue(0L)
            .HasColumnName("wagering_required");

        entity.HasOne(d => d.MaxMultiplierGame).WithMany(p => p.BalanceMaxMultiplierGames)
            .HasForeignKey(d => d.MaxMultiplierGameId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.MaxWinGame).WithMany(p => p.BalanceMaxWinGames)
            .HasForeignKey(d => d.MaxWinGameId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(d => d.User).WithOne(p => p.Balance)
            .HasForeignKey<Balance>(d => d.UserId);
    }
} 