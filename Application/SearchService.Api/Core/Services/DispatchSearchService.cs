using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using OpenSearch.Client;
using SearchService.Api.Core.Interfaces;
using SearchService.Api.Infrastructure.OpenSearch;
using SearchService.Api.Models;
using SearchService.Api.Models.Search;
using System.IdentityModel.Tokens.Jwt;

namespace SearchService.Api;

public class DispatchSearchService(
    IOpenSearchClient client,
    IDispatchSearchQueryBuilder queryBuilder,
    IHttpContextAccessor httpContextAccessor,
    ILogger<DispatchSearchService> logger,
    IOptions<AppSettings> options) : IDispatchSearchService
{
    private readonly string _indexName = options.Value.OpenSearch.IndexName;

    public async Task<(long Total, IEnumerable<Guid> DispatchIds)> SearchAsync(DispatchSearchRequestModel request)
    {
        var token = httpContextAccessor.HttpContext.Request.Headers.Authorization;

        var tenantId = GetTenantId(token);

        // Call to OpenSearch server
        var searchRequest = queryBuilder.BuildOpenSearchRequest(request, tenantId, _indexName);
        var response = await client.SearchAsync<DispatchModel>(searchRequest);

        var logs = response.Documents.Select(d => d.DispatchId);
        Console.WriteLine("============= Open Search =============");
        foreach (var log in logs)
        {
            Console.WriteLine("Open Search Id: {0}", log);
        }

        return (response.Total, response.Documents.Select(d => d.DispatchId));
    }

    private string GetTenantId(string authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader))
            return null;

        var token = AuthenticationHeaderValue.TryParse(authorizationHeader, out var parsed) ? parsed.Parameter : null;

        if (string.IsNullOrWhiteSpace(token))
            return null;

        logger.LogCritical(token);
        var handler = new JwtSecurityTokenHandler();

        var jwt = handler.ReadJwtToken(token);

        var tenantId = jwt.Claims.FirstOrDefault(c => c.Type.Equals("company_id"))?.Value;

        return tenantId;
    }
}
