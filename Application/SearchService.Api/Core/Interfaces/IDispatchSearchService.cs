using SearchService.Api.Models.Search;

namespace SearchService.Api.Core.Interfaces;

public interface IDispatchSearchService
{
    Task<DispatchSearchResponseModel> SearchAsync(DispatchSearchRequestModel requst);
}
