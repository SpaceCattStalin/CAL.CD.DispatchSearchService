using OpenSearch.Client;
using SearchService.Api.Models;
using SearchService.Api.Models.Search;

namespace SearchService.Api;

public interface IDispatchSearchQueryBuilder
{
    SearchRequest<DispatchModel> BuildOpenSearchRequest(DispatchSearchRequestModel request, string indexName);
}
