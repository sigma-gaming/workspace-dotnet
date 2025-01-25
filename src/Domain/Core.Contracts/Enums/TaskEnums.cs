using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GlobalTaskKey
{
    VkGroupSubscribe,
    VkPinnedRepost,
    TelegramGroupSubscribe
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskType
{
    Global,
    Personal
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus
{
    Pending,
    Completed,
    Claimed
}