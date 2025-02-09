using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PromocodeBonusType
{
    DepositMultiplier,
    DepositFixed,
    Payout,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PromocodeUsageStatus
{
    Applied,
    Pending,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PromocodeActivationOutcome
{
    AppliedPayout,
    AppliedDeposit,
    Blocked,
    Expired,
    NotFound,
    Inactive,
    UsageExceeded,
    AlreadyUsed,
    WrongUsage,
    Failed,
}