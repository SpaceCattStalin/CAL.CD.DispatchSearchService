using Microsoft.Extensions.Options;
using Moq;
using OpenSearch.Client;
using SearchService.Api;
using SearchService.Api.Infrastructure.OpenSearch;
using SearchService.Api.Models;
using SearchService.Api.Models.Search;
using SearchService.Api.Tests.TestHelpers;

namespace SearchService.Api.Tests.Core.Services;

public class DispatchSearchServiceTests
{
    private static IOptions<OpenSearchOptions> Options(string indexName = "dispatches") =>
        Microsoft.Extensions.Options.Options.Create(new OpenSearchOptions { IndexName = indexName });

    private const string SearchResponseJson = """
    {
      "took": 1,
      "timed_out": false,
      "_shards": { "total": 1, "successful": 1, "skipped": 0, "failed": 0 },
      "hits": {
        "total": { "value": 5, "relation": "eq" },
        "max_score": 1.0,
        "hits": [
          { "_index": "dispatches", "_id": "1", "_score": 1.0, "_source": { "dispatchId": "11111111-1111-1111-1111-111111111111", "priceTotal": 100.0, "pickupDate": "2026-01-01T00:00:00Z", "dropoffDate": "2026-01-02T00:00:00Z", "dispatchStatus": "Delivered", "vehicle": { "vin": "VIN1" } } },
          { "_index": "dispatches", "_id": "2", "_score": 1.0, "_source": { "dispatchId": "22222222-2222-2222-2222-222222222222", "priceTotal": 200.0, "pickupDate": "2026-01-03T00:00:00Z", "dropoffDate": "2026-01-04T00:00:00Z", "dispatchStatus": "Delivered", "vehicle": { "vin": "VIN2" } } }
        ]
      }
    }
    """;

    [Fact]
    public async Task SearchAsync_ReturnsDispatchIdsFromOpenSearchResponse()
    {
        var client = MockOpenSearchClientFactory.Create(SearchResponseJson, 200);
        var queryBuilder = new Mock<IDispatchSearchQueryBuilder>();
        queryBuilder
            .Setup(b => b.BuildOpenSearchRequest(It.IsAny<DispatchSearchRequestModel>(), It.IsAny<string>()))
            .Returns(new SearchRequest<DispatchModel>("dispatches"));
        var sut = new DispatchSearchService(client, queryBuilder.Object, Options());
        var request = new DispatchSearchRequestModel();

        var response = (await sut.SearchAsync(request)).ToList();

        Assert.Equal(2, response.Count);
        Assert.Contains(Guid.Parse("11111111-1111-1111-1111-111111111111"), response);
        Assert.Contains(Guid.Parse("22222222-2222-2222-2222-222222222222"), response);
    }

    [Fact]
    public async Task SearchAsync_PassesRequestAndIndexNameToQueryBuilder()
    {
        var client = MockOpenSearchClientFactory.Create(SearchResponseJson, 200);
        var queryBuilder = new Mock<IDispatchSearchQueryBuilder>();
        queryBuilder
            .Setup(b => b.BuildOpenSearchRequest(It.IsAny<DispatchSearchRequestModel>(), It.IsAny<string>()))
            .Returns(new SearchRequest<DispatchModel>("dispatches"));
        var sut = new DispatchSearchService(client, queryBuilder.Object, Options("dispatches"));
        var request = new DispatchSearchRequestModel { DispatchStatus = "Delivered" };

        await sut.SearchAsync(request);

        queryBuilder.Verify(b => b.BuildOpenSearchRequest(request, "dispatches"), Times.Once);
    }
}
