namespace SearchService.Api.Models.Search;

public class DispatchSearchRequestModel
{
    public Guid? DispatchId { get; set; }
    public double? PriceTotalMin { get; set; }
    public double? PriceTotalMax { get; set; }
    public DateTime? PickupDateFrom { get; set; }
    public DateTime? PickupDateTo { get; set; }
    public DateTime? DropoffDateFrom { get; set; }
    public DateTime? DropoffDateTo { get; set; }
    public string? DispatchStatus { get; set; }
    public string? VehicleVin { get; set; }
}
