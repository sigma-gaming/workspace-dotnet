using System.Text.Json.Serialization;

namespace Core.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatMessageType
{
    UserMessage,
    SystemMessage
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatMessageAttachmentType
{
    Game
}