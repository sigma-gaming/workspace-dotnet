using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Game
{
    Dice,
    Pincode
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameOutcome
{
    Win,
    Loss
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PincodeMode
{
    Easy,
    Hardcore
}