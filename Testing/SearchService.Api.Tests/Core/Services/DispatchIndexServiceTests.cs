using Microsoft.Extensions.Options;
using SearchService.Api;
using SearchService.Api.Infrastructure.OpenSearch;
using SearchService.Api.Models;
using SearchService.Api.Tests.TestHelpers;

namespace SearchService.Api.Tests.Core.Services;

public class DispatchIndexServiceTests
{
    private static IOptions<OpenSearchOptions> Options() =>
        Microsoft.Extensions.Options.Options.Create(new OpenSearchOptions { IndexName = "dispatches" });

    [Fact]
    public async Task IndexAsync_ValidDocument_ReturnsSuccessWithId()
    {
        const string id = "11111111-1111-1111-1111-111111111111";
        var responseJson = $$"""
        {
          "_index": "dispatches",
          "_id": "{{id}}",
          "_version": 1,
          "result": "created",
          "_shards": { "total": 2, "successful": 1, "failed": 0 },
          "_seq_no": 0,
          "_primary_term": 1
        }
        """;
        var client = MockOpenSearchClientFactory.Create(responseJson, 201);
        var sut = new DispatchIndexService(client, Options());
        var dispatch = new DispatchModel { DispatchId = Guid.Parse(id), DispatchStatus = "Delivered" };

        var result = await sut.IndexAsync(dispatch);

        Assert.True(result.success);
        Assert.Equal(id, result.Id);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task IndexAsync_FailedResponse_ReturnsFailureWithError()
    {
        var responseJson = """
        { "error": { "type": "mapper_parsing_exception", "reason": "failed to parse" }, "status": 400 }
        """;
        var client = MockOpenSearchClientFactory.Create(responseJson, 400);
        var sut = new DispatchIndexService(client, Options());
        var dispatch = new DispatchModel { DispatchId = Guid.NewGuid(), DispatchStatus = "Delivered" };

        var result = await sut.IndexAsync(dispatch);

        Assert.False(result.success);
        Assert.Null(result.Id);
        Assert.False(string.IsNullOrEmpty(result.Error));
    }

    [Fact]
    public async Task DeleteAsync_ExistingDocument_ReturnsSuccess()
    {
        var responseJson = """
        {
          "_index": "dispatches",
          "_id": "x",
          "_version": 2,
          "result": "deleted",
          "_shards": { "total": 2, "successful": 1, "failed": 0 }
        }
        """;
        var client = MockOpenSearchClientFactory.Create(responseJson, 200);
        var sut = new DispatchIndexService(client, Options());

        var result = await sut.DeleteAsync(Guid.NewGuid());

        Assert.True(result.success);
        Assert.False(result.NotFound);
        Assert.Null(result.Error);
    }

    [Fact]
    public async Task DeleteAsync_DocumentDoesNotExist_ReturnsNotFound()
    {
        var responseJson = """
        { "_index": "dispatches", "_id": "x", "_version": 1, "result": "not_found", "_shards": { "total": 2, "successful": 1, "failed": 0 } }
        """;
        var client = MockOpenSearchClientFactory.Create(responseJson, 404);
        var sut = new DispatchIndexService(client, Options());

        var result = await sut.DeleteAsync(Guid.NewGuid());

        Assert.False(result.success);
        Assert.True(result.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_FailedResponse_ReturnsFailureWithoutNotFound()
    {
        var responseJson = """
        { "error": { "type": "some_error", "reason": "boom" }, "status": 500 }
        """;
        var client = MockOpenSearchClientFactory.Create(responseJson, 500);
        var sut = new DispatchIndexService(client, Options());

        var result = await sut.DeleteAsync(Guid.NewGuid());

        Assert.False(result.success);
        Assert.False(result.NotFound);
        Assert.False(string.IsNullOrEmpty(result.Error));
    }
}
