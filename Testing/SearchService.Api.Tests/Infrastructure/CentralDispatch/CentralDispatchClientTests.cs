using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Moq;
using SearchService.Api.Infrastructure.CentralDispatch;
using SearchService.Api.Models.CentralDispatch;

namespace SearchService.Api.Tests.Infrastructure.CentralDispatch;

public class CentralDispatchClientTests
{
    private sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder) : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            LastRequestBody = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            return responder(request);
        }
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string json) =>
        new(statusCode) { Content = new StringContent(json, Encoding.UTF8, "application/json") };

    private static (CentralDispatchClient Sut, FakeHttpMessageHandler Handler) CreateSut(
        Func<HttpRequestMessage, HttpResponseMessage> responder,
        HttpContext? httpContext = null)
    {
        var handler = new FakeHttpMessageHandler(responder);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://central-dispatch.test/") };
        var contextAccessor = new Mock<IHttpContextAccessor>();
        contextAccessor.Setup(a => a.HttpContext).Returns(httpContext!);

        return (new CentralDispatchClient(httpClient, contextAccessor.Object), handler);
    }

    [Fact]
    public async Task GetBatchAsync_SendsPostRequestToBatchGetEndpointWithDispatchIds()
    {
        var ids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var (sut, handler) = CreateSut(_ => JsonResponse(HttpStatusCode.OK, """{"found":[],"notFound":[],"total":0}"""));

        await sut.GetBatchAsync(ids);

        Assert.NotNull(handler.LastRequest);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/api/dispatch/batch/get", handler.LastRequest.RequestUri!.AbsolutePath);
        var body = JsonSerializer.Deserialize<CentralDispatchBatchRequest>(
            handler.LastRequestBody!,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.Equal(ids, body!.DispatchIds);
    }

    [Fact]
    public async Task GetBatchAsync_SuccessResponse_ReordersFoundToMatchRequestedIdOrder()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var responseJson = $$"""
        {
          "found": [
            { "dispatchId": "{{id2}}", "dispatchStatus": "Delivered", "price": 100, "pickupDate": "2026-01-01T00:00:00Z", "dropoffDate": "2026-01-02T00:00:00Z", "isSigned": true, "createdAt": "2026-01-01T00:00:00Z" },
            { "dispatchId": "{{id1}}", "dispatchStatus": "Delivered", "price": 200, "pickupDate": "2026-01-01T00:00:00Z", "dropoffDate": "2026-01-02T00:00:00Z", "isSigned": true, "createdAt": "2026-01-01T00:00:00Z" }
          ],
          "notFound": [],
          "total": 2
        }
        """;
        var (sut, _) = CreateSut(_ => JsonResponse(HttpStatusCode.OK, responseJson));

        var result = await sut.GetBatchAsync([id1, id2]);

        var found = result.Found.ToList();
        Assert.Equal(2, found.Count);
        Assert.Equal(id1, found[0]!.DispatchId);
        Assert.Equal(id2, found[1]!.DispatchId);
        Assert.Equal(2, result.Total);
    }

    [Fact]
    public async Task GetBatchAsync_AuthorizationHeaderPresentOnIncomingRequest_ForwardsItToOutgoingRequest()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers.Authorization = "Bearer abc123";
        var (sut, handler) = CreateSut(
            _ => JsonResponse(HttpStatusCode.OK, """{"found":[],"notFound":[],"total":0}"""),
            context);

        await sut.GetBatchAsync([Guid.NewGuid()]);

        Assert.NotNull(handler.LastRequest!.Headers.Authorization);
        Assert.Equal("Bearer", handler.LastRequest.Headers.Authorization!.Scheme);
        Assert.Equal("abc123", handler.LastRequest.Headers.Authorization.Parameter);
    }

    [Fact]
    public async Task GetBatchAsync_NoHttpContext_DoesNotSetAuthorizationHeaderOnOutgoingRequest()
    {
        var (sut, handler) = CreateSut(
            _ => JsonResponse(HttpStatusCode.OK, """{"found":[],"notFound":[],"total":0}"""),
            httpContext: null);

        await sut.GetBatchAsync([Guid.NewGuid()]);

        Assert.Null(handler.LastRequest!.Headers.Authorization);
    }

    [Fact]
    public async Task GetBatchAsync_NonSuccessStatusCode_ThrowsHttpRequestException()
    {
        var (sut, _) = CreateSut(_ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetBatchAsync([Guid.NewGuid()]));
    }

    [Fact]
    public async Task GetBatchAsync_NullResponseBody_ThrowsNullReferenceException()
    {
        var (sut, _) = CreateSut(_ => JsonResponse(HttpStatusCode.OK, "null"));

        await Assert.ThrowsAsync<NullReferenceException>(() => sut.GetBatchAsync([Guid.NewGuid()]));
    }
}
