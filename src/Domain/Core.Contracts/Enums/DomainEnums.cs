using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DomainApp
{
    CoreApp,
    CoreApi,
    ControlApp,
}