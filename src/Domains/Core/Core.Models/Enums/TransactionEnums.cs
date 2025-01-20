using System.Text.Json.Serialization;

namespace Core.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
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
