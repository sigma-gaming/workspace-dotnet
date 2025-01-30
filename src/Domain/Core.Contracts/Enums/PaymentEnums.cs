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
    Expired
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
    Doge
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DepositType
{
    Redirect,
    WhiteLabel
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DepositMethod
{
    Sbp,
    CreditCard,
    Piastrix,
    Toncoin
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WithdrawalMethod
{
    Sbp,
    CreditCard,
    Piastrix
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentProvider
{
    Bovapay,
    Test
}