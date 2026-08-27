using SearchService.Api.Models.Enums;

namespace SearchService.Api.Models;

public class DispatchWriterEvent(
    EventType Type,
    Guid DispatchId,
    decimal PriceTotal,
    DateTime PickupDate,
    DateTime DropoffDate,
    DispatchStatus DispatchStatus,
    IEnumerable<DispatchWriterVehicle> Vehicles)
{
    public EventType Type { get; } = Type;
    public Guid DispatchId { get; } = DispatchId;
    public decimal PriceTotal { get; } = PriceTotal;
    public DateTime PickupDate { get; } = PickupDate;
    public DateTime DropoffDate { get; } = DropoffDate;
    public DispatchStatus DispatchStatus { get; } = DispatchStatus;
    public IEnumerable<DispatchWriterVehicle> Vehicles { get; } = Vehicles;
}

public class DispatchWriterVehicle(string? Vin)
{
    public string? Vin { get; } = Vin;
}
