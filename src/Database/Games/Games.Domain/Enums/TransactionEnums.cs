namespace Games.Domain.Enums;

public enum TransactionType
{
    Deposit,
    Withdrawal,
    Transfer,
    Bonus,
    Bet,
    Win,
    Loss,
    Refund
}

public enum FraudRisk
{
    Clear,
    Low,
    Medium,
    High,
    Unknown
} 