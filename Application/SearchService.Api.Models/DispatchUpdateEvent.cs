using SearchService.Api.Models.Enums;

namespace SearchService.Api.Models;

public record class DispatchUpdateEvent(
    EventType Type,
    Guid DispatchId,
    Guid CarrierId,
    Guid ShipperId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchUpdateVehicle> Vehicles);

public record class DispatchUpdateVehicle(string? Vin);