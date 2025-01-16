namespace Games.Domain.Enums;

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

public enum DepositType
{
    Redirect,
    WhiteLabel
}

public enum DepositMethod
{
    SBP,
    CreditCard,
    Piastrix,
    Toncoin
}

public enum WithdrawalMethod
{
    SBP,
    CreditCard,
    Piastrix
}

public enum PaymentProvider
{
    Bovapay,
    Test
} 