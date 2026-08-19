using Microsoft.Extensions.Options;
using OpenSearch.Client;
using SearchService.Api.Core.Interfaces;
using SearchService.Api.Infrastructure.OpenSearch;
using SearchService.Api.Models;
using SearchService.Api.Models.Search;

namespace SearchService.Api;

public class DispatchSearchService(
    IOpenSearchClient client,
    IDispatchSearchQueryBuilder queryBuilder,
    IOptions<OpenSearchOptions> options) : IDispatchSearchService
{
    private readonly string _indexName = options.Value.IndexName;

    public async Task<DispatchSearchResponseModel> SearchAsync(DispatchSearchRequestModel request)
    {
        var searchRequest = queryBuilder.BuildOpenSearchRequest(request, _indexName);
        var response = await client.SearchAsync<DispatchModel>(searchRequest);

        return new DispatchSearchResponseModel
        {
            Total = response.Total,
            Page = Math.Max(1, request.Page),
            PageSize = Math.Clamp(request.PageSize, 1, 100),
            Items = response.Documents.ToList()
        };
    }
}
