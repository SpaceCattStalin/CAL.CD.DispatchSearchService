namespace SearchService.Api.Models.CentralDispatch;

public class CentralDispatchVehicleResponse
{
    public Guid VehicleId { get; init; }
    public string VehicleStatus { get; init; } = string.Empty;
    public string? Vin { get; init; }
    public int Year { get; init; }
    public string Make { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string? Color { get; init; }
    public CentralDispatchStopResponse? PickupStop { get; init; }
    public CentralDispatchStopResponse? DropoffStop { get; init; }
}
