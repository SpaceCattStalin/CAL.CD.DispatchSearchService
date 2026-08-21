using SearchService.Api.Models.Search;

namespace SearchService.Api.Core.Interfaces;

public interface IDispatchSearchService
{
    Task<IEnumerable<Guid>> SearchAsync(DispatchSearchRequestModel requst);
}
