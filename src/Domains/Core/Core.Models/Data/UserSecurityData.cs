using System;

namespace Core.Domain.Data;

public sealed record UserSecurityData
{
    public string? LastIp { get; init; }
    public string? AddressUsdt { get; init; }
    public string? AddressUsdc { get; init; }
    public string? AddressTrx { get; init; }
    public string? AddressEth { get; init; }
    public string? AddressBtc { get; init; }
    public string? AddressLtc { get; init; }
    public string? AddressTon { get; init; }
    public string? AddressDoge { get; init; }
    public string? AddressBnb { get; init; }
    public string? AddressXmr { get; init; }
    public string? AddressSol { get; init; }
    public required Guid UserId { get; init; }
    public required bool Whitelisted { get; init; }
    public required int IpScore { get; init; }
    public required int AddressTonScore { get; init; }
    public required int AddressUsdtScore { get; init; }
    public required int AddressUsdcScore { get; init; }
    public required int AddressTrxScore { get; init; }
    public required int AddressEthScore { get; init; }
    public required int AddressBtcScore { get; init; }
    public required int AddressLtcScore { get; init; }
    public required int AddressDogeScore { get; init; }
    public required int AddressBnbScore { get; init; }
    public required int AddressXmrScore { get; init; }
    public required int AddressSolScore { get; init; }
} 