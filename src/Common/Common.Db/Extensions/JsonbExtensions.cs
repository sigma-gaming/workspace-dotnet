using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Common.Db.Extensions;

public static class JsonbSerializerOptions
{
    public static JsonSerializerOptions Options { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowOutOfOrderMetadataProperties = true,
    };
}

public static class JsonbExtensions
{
    public static PropertyBuilder<TValue> HasJsonbConversion<TValue>(
        this PropertyBuilder<TValue> builder)
    {
        return builder.HasConversion(
            v => JsonSerializer.Serialize(v, JsonbSerializerOptions.Options),
            v => JsonSerializer.Deserialize<TValue>(v, JsonbSerializerOptions.Options)!);
    }
}