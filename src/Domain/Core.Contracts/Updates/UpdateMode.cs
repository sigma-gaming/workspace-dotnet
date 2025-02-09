using System.Text.Json.Serialization;

namespace Core.Contracts.Updates;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UpdateMode
{
    Always,
    Optimized,
}