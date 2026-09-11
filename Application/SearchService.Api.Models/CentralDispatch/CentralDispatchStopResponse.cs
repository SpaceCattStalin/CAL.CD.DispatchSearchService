namespace SearchService.Api.Models.CentralDispatch;

public class CentralDispatchStopResponse
{
    public Guid StopId { get; init; }
    public string StopNumber { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string? LocationName { get; init; }
    public string? ContactName { get; init; }
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
}
