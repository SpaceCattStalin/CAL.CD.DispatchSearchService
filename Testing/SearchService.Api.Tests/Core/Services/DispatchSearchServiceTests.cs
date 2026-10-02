using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
    private static IOptions<AppSettings> Options(string indexName = "dispatches") =>
        Microsoft.Extensions.Options.Options.Create(new AppSettings
        {
            OpenSearch = new OpenSearchSettings
            {
                Uri = "https://localhost:9200",
                Username = "test",
                Password = "test",
                IndexName = indexName
            },
            Jwt = new JwtSettings
            {
                Issuer = "CentralDispatch",
                Audience = "CentralDispatch",
                SigningKey = "test_signing_key_min_32_chars_long_0000"
            },
            CentralDispatch = new CentralDispatchSettings
            {
                BaseUrl = "https://central-dispatch.test"
            }
        });

    private const string SearchResponseJson = """
    {
      "took": 1,
      "timed_out": false,
      "_shards": { "total": 1, "successful": 1, "skipped": 0, "failed": 0 },
      "hits": {
        "total": { "value": 5, "relation": "eq" },
        "max_score": 1.0,
        "hits": [
          { "_index": "dispatches", "_id": "1", "_score": 1.0, "_source": { "dispatchId": "11111111-1111-1111-1111-111111111111", "priceTotal": 100.0, "pickupDate": "2026-01-01T00:00:00Z", "dropoffDate": "2026-01-02T00:00:00Z", "dispatchStatus": "Delivered" } },
          { "_index": "dispatches", "_id": "2", "_score": 1.0, "_source": { "dispatchId": "22222222-2222-2222-2222-222222222222", "priceTotal": 200.0, "pickupDate": "2026-01-03T00:00:00Z", "dropoffDate": "2026-01-04T00:00:00Z", "dispatchStatus": "Delivered" } }
        ]
      }
    }
    """;

    private static DispatchSearchService CreateSut(HttpContext httpContext, out Mock<IDispatchSearchQueryBuilder> queryBuilder)
    {
        var client = MockOpenSearchClientFactory.Create(SearchResponseJson, 200);
        queryBuilder = new Mock<IDispatchSearchQueryBuilder>();
        queryBuilder
            .Setup(b => b.BuildOpenSearchRequest(It.IsAny<DispatchSearchRequestModel>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new SearchRequest<DispatchModel>("dispatches"));

        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(a => a.HttpContext).Returns(httpContext);

        return new DispatchSearchService(
            client,
            queryBuilder.Object,
            contextAccessor.Object,
            Mock.Of<ILogger<DispatchSearchService>>(),
            Options());
    }

    private static DefaultHttpContext ContextWithAuthHeader(string? headerValue)
    {
        var context = new DefaultHttpContext();
        if (headerValue is not null)
            context.Request.Headers.Authorization = headerValue;
        return context;
    }

    private static string CreateJwt(string? companyId)
    {
        var claims = companyId is null
            ? []
            : new[] { new Claim("company_id", companyId) };
        var token = new JwtSecurityToken(claims: claims);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task SearchAsync_ReturnsTotalAndDispatchIdsFromOpenSearchResponse()
    {
        var sut = CreateSut(ContextWithAuthHeader(null), out _);

        var (total, dispatchIds) = await sut.SearchAsync(new DispatchSearchRequestModel());

        Assert.Equal(5, total);
        var ids = dispatchIds.ToList();
        Assert.Equal(2, ids.Count);
        Assert.Contains(Guid.Parse("11111111-1111-1111-1111-111111111111"), ids);
        Assert.Contains(Guid.Parse("22222222-2222-2222-2222-222222222222"), ids);
    }

    [Fact]
    public async Task SearchAsync_PassesRequestAndIndexNameToQueryBuilder()
    {
        var sut = CreateSut(ContextWithAuthHeader(null), out var queryBuilder);
        var request = new DispatchSearchRequestModel { DispatchStatus = ["Delivered"] };

        await sut.SearchAsync(request);

        queryBuilder.Verify(b => b.BuildOpenSearchRequest(request, It.IsAny<string>(), "dispatches"), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_NoAuthorizationHeader_PassesNullTenantIdToQueryBuilder()
    {
        var sut = CreateSut(ContextWithAuthHeader(null), out var queryBuilder);

        await sut.SearchAsync(new DispatchSearchRequestModel());

        queryBuilder.Verify(
            b => b.BuildOpenSearchRequest(It.IsAny<DispatchSearchRequestModel>(), null!, It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task SearchAsync_AuthorizationHeaderWithoutBearerToken_PassesNullTenantIdToQueryBuilder()
    {
        var sut = CreateSut(ContextWithAuthHeader("NotBearerFormat"), out var queryBuilder);

        await sut.SearchAsync(new DispatchSearchRequestModel());

        queryBuilder.Verify(
            b => b.BuildOpenSearchRequest(It.IsAny<DispatchSearchRequestModel>(), null!, It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ValidJwtWithCompanyIdClaim_PassesTenantIdToQueryBuilder()
    {
        var companyId = Guid.NewGuid().ToString();
        var jwt = CreateJwt(companyId);
        var sut = CreateSut(ContextWithAuthHeader($"Bearer {jwt}"), out var queryBuilder);

        await sut.SearchAsync(new DispatchSearchRequestModel());

        queryBuilder.Verify(
            b => b.BuildOpenSearchRequest(It.IsAny<DispatchSearchRequestModel>(), companyId, It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ValidJwtWithoutCompanyIdClaim_PassesNullTenantIdToQueryBuilder()
    {
        var jwt = CreateJwt(companyId: null);
        var sut = CreateSut(ContextWithAuthHeader($"Bearer {jwt}"), out var queryBuilder);

        await sut.SearchAsync(new DispatchSearchRequestModel());

        queryBuilder.Verify(
            b => b.BuildOpenSearchRequest(It.IsAny<DispatchSearchRequestModel>(), null!, It.IsAny<string>()),
            Times.Once);
    }
}
