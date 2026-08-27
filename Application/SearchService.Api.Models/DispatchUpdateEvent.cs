using SearchService.Api.Models.Enums;

namespace SearchService.Api.Models;

public class DispatchUpdateEvent(
    EventType Type,
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchUpdateVehicle> Vehicles)
{
    public EventType Type { get; } = Type;
    public Guid DispatchId { get; } = DispatchId;
    public decimal PriceTotal { get; } = PriceTotal;
    public DateTime PickupDate { get; } = PickupDate;
    public DateTime DropoffDate { get; } = DropoffDate;
    public DispatchStatus DispatchStatus { get; } = DispatchStatus;
    public IEnumerable<DispatchUpdateVehicle> Vehicles { get; } = Vehicles;
}

public class DispatchUpdateVehicle(string? Vin)
{
    public string? Vin { get; } = Vin;
}