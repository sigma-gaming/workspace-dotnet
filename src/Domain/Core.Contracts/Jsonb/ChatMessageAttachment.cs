using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(ChatMessageAttachmentGame), (int)ChatMessageAttachmentType.Game)]
public abstract record ChatMessageAttachment
{
    [Required] public abstract ChatMessageAttachmentType Type { get; }
}

public sealed record ChatMessageAttachmentGame : ChatMessageAttachment
{
    [Required] public override ChatMessageAttachmentType Type => ChatMessageAttachmentType.Game;
    public required Guid GameRecordId { get; init; }
}