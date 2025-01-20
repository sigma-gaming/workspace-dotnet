using System.Text.Json.Serialization;

namespace Core.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PromocodeBonusType
{
    DepositMultiplier,
    DepositFixed,
    Payout
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PromocodeUsageStatus
{
    Applied,
    Pending
}