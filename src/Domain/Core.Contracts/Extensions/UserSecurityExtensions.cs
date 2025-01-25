using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class UserSecurityExtensions
{
    public static UserSecurity ToData(this UserSecurityEntity entity)
    {
        return new UserSecurity
        {
            LastIp = entity.LastIp,
            AddressUsdt = entity.AddressUsdt,
            AddressUsdc = entity.AddressUsdc,
            AddressTrx = entity.AddressTrx,
            AddressEth = entity.AddressEth,
            AddressBtc = entity.AddressBtc,
            AddressLtc = entity.AddressLtc,
            AddressTon = entity.AddressTon,
            AddressDoge = entity.AddressDoge,
            AddressBnb = entity.AddressBnb,
            AddressXmr = entity.AddressXmr,
            AddressSol = entity.AddressSol,
            UserId = entity.UserId,
            Whitelisted = entity.Whitelisted,
            IpScore = entity.IpScore,
            AddressTonScore = entity.AddressTonScore,
            AddressUsdtScore = entity.AddressUsdtScore,
            AddressUsdcScore = entity.AddressUsdcScore,
            AddressTrxScore = entity.AddressTrxScore,
            AddressEthScore = entity.AddressEthScore,
            AddressBtcScore = entity.AddressBtcScore,
            AddressLtcScore = entity.AddressLtcScore,
            AddressDogeScore = entity.AddressDogeScore,
            AddressBnbScore = entity.AddressBnbScore,
            AddressXmrScore = entity.AddressXmrScore,
            AddressSolScore = entity.AddressSolScore
        };
    }
} 