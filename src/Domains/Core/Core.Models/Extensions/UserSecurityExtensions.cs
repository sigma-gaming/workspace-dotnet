using Core.Domain.Data;
using Core.Domain.Entities;

namespace Core.Domain.Entities;

public static class UserSecurityExtensions
{
    public static UserSecurityData ToData(this UserSecurity entity)
    {
        return new UserSecurityData
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