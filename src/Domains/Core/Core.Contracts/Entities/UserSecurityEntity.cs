namespace Core.Contracts.Entities;

public sealed record UserSecurityEntity
{
    public string? LastIp { get; set; }

    public string? AddressUsdt { get; set; }

    public string? AddressUsdc { get; set; }

    public string? AddressTrx { get; set; }

    public string? AddressEth { get; set; }

    public string? AddressBtc { get; set; }

    public string? AddressLtc { get; set; }

    public string? AddressTon { get; set; }

    public string? AddressDoge { get; set; }

    public string? AddressBnb { get; set; }

    public string? AddressXmr { get; set; }

    public string? AddressSol { get; set; }

    public Guid UserId { get; set; }

    public bool Whitelisted { get; set; }

    public int IpScore { get; set; }

    public int AddressTonScore { get; set; }

    public int AddressUsdtScore { get; set; }

    public int AddressUsdcScore { get; set; }

    public int AddressTrxScore { get; set; }

    public int AddressEthScore { get; set; }

    public int AddressBtcScore { get; set; }

    public int AddressLtcScore { get; set; }

    public int AddressDogeScore { get; set; }

    public int AddressBnbScore { get; set; }

    public int AddressXmrScore { get; set; }

    public int AddressSolScore { get; set; }

    public UserEntity User { get; set; } = null!;
}
