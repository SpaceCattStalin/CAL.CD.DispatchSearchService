using System.Text;
using OpenSearch.Client;
using OpenSearch.Net;

namespace SearchService.Api.Tests.TestHelpers;

/// <summary>
/// Compute IsValid from internal transport
/// state, so they can't be hand-built as "valid" via object initializers. InMemoryConnection lets
/// a real OpenSearchClient run through real (de)serialization against a canned response instead
/// of a network call, producing genuinely valid response objects.
/// </summary>
internal static class MockOpenSearchClientFactory
{
    public static IOpenSearchClient Create(string responseJson, int statusCode, string indexName = "dispatches")
    {
        var connection = new InMemoryConnection(Encoding.UTF8.GetBytes(responseJson), statusCode, null, "application/json");
        var pool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
        var settings = new ConnectionSettings(pool, connection).DefaultIndex(indexName);
        return new OpenSearchClient(settings);
    }
}
