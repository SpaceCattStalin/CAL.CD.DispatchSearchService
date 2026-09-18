namespace SearchService.Api.Models;

public class DispatchModel
{
    public Guid DispatchId { get; set; } = Guid.Empty;
    public Guid CarrierId { get; set; } = Guid.Empty;
    public Guid ShipperId { get; set; } = Guid.Empty;
    public double PriceTotal { get; set; }
    public DateTime PickupDate { get; set; }
    public DateTime DropoffDate { get; set; }
    public string DispatchStatus { get; set; } = string.Empty;
    public List<VehicleModel> Vehicles { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}
