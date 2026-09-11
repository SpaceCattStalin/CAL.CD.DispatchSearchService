using SearchService.Api.Models.CentralDispatch;

namespace SearchService.Api.Core.Interfaces;

public interface ICentralDispatchClient
{
    Task<CentralDispatchBatchResponse> GetBatchAsync(IEnumerable<Guid> dispatchIds, CancellationToken cancellationToken = default);
}
