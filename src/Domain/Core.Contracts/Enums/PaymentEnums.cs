using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Rejected,
    Cancelled,
    Expired,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Currency
{
    Rub,
    Kzt,
    Kgs,
    Uzs,
    Uah,
    Usd,
    Eur,
    Trx,
    UsdtTrc20,
    UsdtErc20,
    Btc,
    Ltc,
    Ton,
    Not,
    Eth,
    Bnb,
    Doge,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DepositType
{
    Redirect,
    WhiteLabel,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DepositMethod
{
    Crypto,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WithdrawalMethod
{
    Crypto,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentProvider
{
    Plisio,
    CryptoCloud,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DepositInstructionsType
{
    CryptoWallet,
}