namespace SearchService.Api.Models.CentralDispatch;

public class CentralDispatchBatchResponse
{
    public IEnumerable<CentralDispatchDispatchResponse> Found { get; init; } = [];
    public IEnumerable<Guid> NotFound { get; init; } = [];
    public long Total { get; init; }
}
