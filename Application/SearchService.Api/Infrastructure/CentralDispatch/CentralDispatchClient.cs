using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using SearchService.Api.Core.Interfaces;
using SearchService.Api.Models.CentralDispatch;

namespace SearchService.Api.Infrastructure.CentralDispatch;

public class CentralDispatchClient(
    HttpClient httpClient,
    IHttpContextAccessor httpContextAccessor) : ICentralDispatchClient
{
    public async Task<CentralDispatchBatchResponse> GetBatchAsync(IEnumerable<Guid> dispatchIds, CancellationToken cancellationToken = default)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/dispatch/batch/get")
        {
            Content = JsonContent.Create(new CentralDispatchBatchRequest { DispatchIds = dispatchIds })
        };

        // Forward the frontend caller's bearer token so CentralDispatch's [Authorize(Policy = DispatchesRead)] check passes.
        var incomingAuthHeader = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrEmpty(incomingAuthHeader) && AuthenticationHeaderValue.TryParse(incomingAuthHeader, out var authHeader))
            httpRequest.Headers.Authorization = authHeader;

        var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);

        Console.WriteLine($"CentralDispatch response: {(int)httpResponse.StatusCode} {httpResponse.StatusCode}");
        Console.WriteLine(await httpResponse.Content.ReadAsStringAsync(cancellationToken));

        httpResponse.EnsureSuccessStatusCode();

        var result = await httpResponse.Content.ReadFromJsonAsync<CentralDispatchBatchResponse>(cancellationToken);
        return result ?? new CentralDispatchBatchResponse();
    }
}
