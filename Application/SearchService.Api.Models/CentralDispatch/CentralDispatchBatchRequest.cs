namespace SearchService.Api.Models.CentralDispatch;

public class CentralDispatchBatchRequest
{
    public IEnumerable<Guid> DispatchIds { get; init; } = [];
}
