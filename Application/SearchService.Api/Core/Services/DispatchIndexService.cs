using Microsoft.Extensions.Options;
using OpenSearch.Client;
using SearchService.Api.Core.Interfaces;
using SearchService.Api.Infrastructure.OpenSearch;
using SearchService.Api.Models;

namespace SearchService.Api;

public class DispatchIndexService(IOpenSearchClient client, IOptions<OpenSearchOptions> options) : IDispatchIndexService
{
    private readonly string _indexName = options.Value.IndexName;

    public async Task<DispatchIndexResult> IndexAsync(DispatchModel dispatch)
    {
        var response = await client.IndexAsync(dispatch,
            i => i.Index(_indexName).Id(dispatch.DispatchId));

        return response.IsValid
            ? new DispatchIndexResult(true, response.Id, null)
            : new DispatchIndexResult(false, null, response.DebugInformation);
    }

    public async Task<DispatchDeleteResult> DeleteAsync(Guid dispatchId)
    {
        var response = await client.DeleteAsync<DispatchModel>(dispatchId,
            d => d.Index(_indexName));

        if (response.Result == Result.NotFound)
            return new DispatchDeleteResult(false, true, null);

        return response.IsValid
            ? new DispatchDeleteResult(true, false, null)
            : new DispatchDeleteResult(false, false, response.DebugInformation);
    }
}
