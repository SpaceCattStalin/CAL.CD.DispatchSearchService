using System.Text.Json.Serialization;

namespace SearchService.Api.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventType
{
    Create,
    Delete,
    Update
}
