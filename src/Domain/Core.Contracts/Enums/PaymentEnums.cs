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
    RUB,
    KZT,
    KGS,
    UZS,
    UAH,
    USD,
    EUR,
    TRX,
    USDT_TRC20,
    USDT_ERC20,
    BTC,
    LTC,
    TON,
    NOT,
    ETH,
    BNB,
    DOGE
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
    SBP,
    CreditCard,
    Piastrix,
    Toncoin
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WithdrawalMethod
{
    SBP,
    CreditCard,
    Piastrix
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentProvider
{
    Bovapay,
    Test
}