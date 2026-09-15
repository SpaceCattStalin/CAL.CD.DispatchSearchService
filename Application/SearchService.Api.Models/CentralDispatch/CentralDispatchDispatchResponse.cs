namespace SearchService.Api.Models.CentralDispatch;

public class CentralDispatchDispatchResponse
{
    public Guid DispatchId { get; init; }
    public Guid ShipperId { get; init; }
    public Guid CarrierId { get; init; }
    public string DispatchStatus { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public DateTime PickupDate { get; init; }
    public DateTime DropoffDate { get; init; }
    public string? Description { get; init; }
    public bool IsSigned { get; init; }
    public CentralDispatchStopResponse? PickupStop { get; init; }
    public CentralDispatchStopResponse? DropoffStop { get; init; }
    public string? CarrierCompanyName { get; init; }
    public string? CarrierCompanyPhone { get; init; }
    public string? CarrierCompanyEmail { get; init; }
    public IEnumerable<CentralDispatchVehicleResponse> Vehicles { get; init; } = [];
    public IEnumerable<CentralDispatchDriverResponse> Drivers { get; init; } = [];
    public DateTime CreatedAt { get; init; }
}
