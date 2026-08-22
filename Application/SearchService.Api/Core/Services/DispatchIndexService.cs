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

        // var response = await client.IndexAsync(dispatch,
        //     i => i.Index(_indexName).Id(dispatch.DispatchId));

        // Include .Id(dispatch.DispatchId) will make the OpenSearchClient call 
        // OpenSearch server with the PUT action. This will make the OpenSearch server to 
        // assign the bussiness dispatch id as the document. This will allow the 
        // later update call to perform an `upsert` 
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

    public async Task<DispatchUpdateResult> UpdateAsync(DispatchModel dispatch)
    {
        // Include .Id(dispatch.DispatchId) will make the OpenSearchClient call 
        // OpenSearch server with the PUT action. This will make the OpenSearch server to 
        // find and update the document
        var response = await client.IndexAsync(dispatch,
            i => i.Index(_indexName).Id(dispatch.DispatchId));


        return response.IsValid
            ? new DispatchUpdateResult(true, null)
            : new DispatchUpdateResult(false, response.DebugInformation);
    }
}
