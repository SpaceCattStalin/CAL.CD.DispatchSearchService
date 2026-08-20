using Microsoft.Extensions.Options;
using OpenSearch.Client;
using SearchService.Api.Models;

namespace SearchService.Api.Infrastructure.OpenSearch;

public class DispatchIndexInitializer(IOpenSearchClient client, IOptions<OpenSearchOptions> options)
{
    private readonly string _indexName = options.Value.IndexName;

    public async Task EnsureIndexAsync(CancellationToken ct = default)
    {
        var exists = await client.Indices.ExistsAsync(_indexName, ct: ct);
        if (exists.Exists)
            return;

        await client.Indices.CreateAsync(_indexName, c => c
            .Map<DispatchModel>(m => m
                .Properties(p => p
                    .Keyword(k => k.Name(d => d.DispatchId))
                    .Keyword(k => k.Name(d => d.DispatchStatus))
                    .Number(n => n.Name(d => d.PriceTotal).Type(NumberType.Double))
                    .Date(dt => dt.Name(d => d.PickupDate))
                    .Date(dt => dt.Name(d => d.DropoffDate))
                    .Object<VehicleModel>(o => o
                        .Name(d => d.Vehicles)
                        .Properties(vp => vp
                            .Keyword(k => k.Name(v => v.Vin)))))), ct);
    }
}
