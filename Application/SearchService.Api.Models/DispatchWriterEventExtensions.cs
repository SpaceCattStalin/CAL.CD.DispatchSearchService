namespace SearchService.Api.Models;

public static class DispatchWriterEventExtensions
{
    public static DispatchModel ToDispatchModel(this DispatchWriterEvent dispatchEvent) => new()
    {
        DispatchId = dispatchEvent.DispatchId,
        PriceTotal = (double)dispatchEvent.PriceTotal,
        PickupDate = dispatchEvent.PickupDate,
        DropoffDate = dispatchEvent.DropoffDate,
        DispatchStatus = dispatchEvent.DispatchStatus.ToString(),
        Vehicles = dispatchEvent.Vehicles
            .Select(v => new VehicleModel { Vin = v.Vin ?? string.Empty })
            .ToList()
    };
}
