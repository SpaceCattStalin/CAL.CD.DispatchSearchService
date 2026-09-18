using System.Text.Json.Serialization;

namespace SearchService.Api.Models.Search;

public class DispatchSearchRequestModel
{
    public double? PriceTotalMin { get; set; }
    public double? PriceTotalMax { get; set; }
    public DateTime? PickupDateFrom { get; set; }
    public DateTime? PickupDateTo { get; set; }
    public DateTime? DropoffDateFrom { get; set; }
    public DateTime? DropoffDateTo { get; set; }
    public string[]? DispatchStatus { get; set; }
    public string? VehicleVin { get; set; }
    public int? Size { get; set; } = 50;
    public int? CurrentPage { get; set; } = 0;
    public ICollection<SortFields> SortFields { get; set; } = [];
}

public class SortFields
{
    public string Name { get; set; }
    public SortDirection Direction { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortDirection
{
    ASCENDING,
    DESCENDING
}