using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReferralAction
{
    Deposit,
    Withdrawal,
}