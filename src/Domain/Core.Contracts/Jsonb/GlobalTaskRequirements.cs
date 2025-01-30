using System.Text.Json.Serialization;
using Core.Contracts.Enums;

namespace Core.Contracts.Jsonb;

[JsonDerivedType(typeof(GlobalTaskRequirementsVkGroupSubscribe),
    typeDiscriminator: (int)GlobalTaskKey.VkGroupSubscribe)]
[JsonDerivedType(typeof(GlobalTaskRequirementsVkPinnedRepost),
    typeDiscriminator: (int)GlobalTaskKey.VkPinnedRepost)]
[JsonDerivedType(typeof(GlobalTaskRequirementsTelegramGroupSubscribe),
    typeDiscriminator: (int)GlobalTaskKey.TelegramGroupSubscribe)]
public abstract record GlobalTaskRequirements
{
    public abstract GlobalTaskKey Type { get; }
};

public sealed record GlobalTaskRequirementsVkGroupSubscribe : GlobalTaskRequirements
{
    public override GlobalTaskKey Type => GlobalTaskKey.VkGroupSubscribe;
    public required string GroupHandle { get; init; }
    public required int GroupId { get; init; }
}

public sealed record GlobalTaskRequirementsVkPinnedRepost : GlobalTaskRequirements
{
    public override GlobalTaskKey Type => GlobalTaskKey.VkPinnedRepost;
    public required string GroupHandle { get; init; }
    public required int GroupId { get; init; }
    public required int PostId { get; init; }
};

public sealed record GlobalTaskRequirementsTelegramGroupSubscribe : GlobalTaskRequirements
{
    public override GlobalTaskKey Type => GlobalTaskKey.TelegramGroupSubscribe;
    public required string GroupHandle { get; init; }
    public required int GroupId { get; init; }
};