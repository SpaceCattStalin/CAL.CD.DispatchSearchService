using System.Text.Json.Serialization;

namespace SearchService.Api.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DispatchStatus
{
    NotSigned,
    PendingPickup,
    PendingDelivery,
    Delivered,
    Canceled
}
