using SearchService.Api.Models.Search;

namespace SearchService.Api.Core.Interfaces;

public interface IDispatchSearchService
{
    Task<(long Total, IEnumerable<Guid> DispatchIds)> SearchAsync(DispatchSearchRequestModel request);
}
