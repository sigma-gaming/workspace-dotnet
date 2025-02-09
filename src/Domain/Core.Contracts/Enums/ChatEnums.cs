using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatMessageType
{
    UserMessage,
    SystemMessage,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChatMessageAttachmentType
{
    Game,
}