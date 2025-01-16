namespace Games.Domain.Enums;

public enum GlobalTaskKey
{
    VkGroupSubscribe,
    VkPinnedRepost,
    TelegramGroupSubscribe
}

public enum TaskType
{
    Global,
    Personal
}

public enum TaskStatus
{
    Pending,
    Completed,
    Claimed
} 