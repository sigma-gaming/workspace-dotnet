using Core.Contracts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.Db.Configurations;

public class UserSecurityConfiguration : IEntityTypeConfiguration<UserSecurityEntity>
{
    public void Configure(EntityTypeBuilder<UserSecurityEntity> entity)
    {
        entity.HasKey(e => e.UserId);

        entity.ToTable("user_security");

        entity.HasIndex(e => e.LastIp);

        entity.Property(e => e.UserId)
            .ValueGeneratedNever()
            .HasColumnName("user_id");
        entity.Property(e => e.AddressBnb).HasColumnName("address_bnb");
        entity.Property(e => e.AddressBnbScore)
            .HasDefaultValue(0)
            .HasColumnName("address_bnb_score");
        entity.Property(e => e.AddressBtc).HasColumnName("address_btc");
        entity.Property(e => e.AddressBtcScore)
            .HasDefaultValue(0)
            .HasColumnName("address_btc_score");
        entity.Property(e => e.AddressDoge).HasColumnName("address_doge");
        entity.Property(e => e.AddressDogeScore)
            .HasDefaultValue(0)
            .HasColumnName("address_doge_score");
        entity.Property(e => e.AddressEth).HasColumnName("address_eth");
        entity.Property(e => e.AddressEthScore)
            .HasDefaultValue(0)
            .HasColumnName("address_eth_score");
        entity.Property(e => e.AddressLtc).HasColumnName("address_ltc");
        entity.Property(e => e.AddressLtcScore)
            .HasDefaultValue(0)
            .HasColumnName("address_ltc_score");
        entity.Property(e => e.AddressSol).HasColumnName("address_sol");
        entity.Property(e => e.AddressSolScore)
            .HasDefaultValue(0)
            .HasColumnName("address_sol_score");
        entity.Property(e => e.AddressTon).HasColumnName("address_ton");
        entity.Property(e => e.AddressTonScore)
            .HasDefaultValue(0)
            .HasColumnName("address_ton_score");
        entity.Property(e => e.AddressTrx).HasColumnName("address_trx");
        entity.Property(e => e.AddressTrxScore)
            .HasDefaultValue(0)
            .HasColumnName("address_trx_score");
        entity.Property(e => e.AddressUsdc).HasColumnName("address_usdc");
        entity.Property(e => e.AddressUsdcScore)
            .HasDefaultValue(0)
            .HasColumnName("address_usdc_score");
        entity.Property(e => e.AddressUsdt).HasColumnName("address_usdt");
        entity.Property(e => e.AddressUsdtScore)
            .HasDefaultValue(0)
            .HasColumnName("address_usdt_score");
        entity.Property(e => e.AddressXmr).HasColumnName("address_xmr");
        entity.Property(e => e.AddressXmrScore)
            .HasDefaultValue(0)
            .HasColumnName("address_xmr_score");
        entity.Property(e => e.IpScore)
            .HasDefaultValue(0)
            .HasColumnName("ip_score");
        entity.Property(e => e.LastIp).HasColumnName("last_ip");
        entity.Property(e => e.Whitelisted)
            .HasDefaultValue(false)
            .HasColumnName("whitelisted");

        entity.HasOne(d => d.User).WithOne(p => p.UserSecurity)
            .HasForeignKey<UserSecurityEntity>(d => d.UserId);
    }
} 