using OpenSearch.Client;
using SearchService.Api.Models;
using SearchService.Api.Models.Search;

namespace SearchService.Api.Core.Search;

public class DispatchSearchQueryBuilder : IDispatchSearchQueryBuilder
{
    public DispatchSearchQueryBuilder()
    {

    }
    /// <summary>
    /// Builds an OpenSearch <see cref="SearchRequest{TDocument}"/> for querying dispatches,
    /// translating the provided filters into query clauses (dispatch ID, status, price range,
    /// pickup/dropoff date ranges, vehicle VIN) and applying paging.
    /// </summary>
    /// <param name="request">The dispatch search filters and paging options to translate into a query.</param>
    /// <param name="indexName">The name of the OpenSearch index to target.</param>
    /// <returns>Return object of type SearchRequest of OpenSearch.Client</returns>
    public SearchRequest<DispatchModel> BuildOpenSearchRequest(DispatchSearchRequestModel request, string indexName) 
    {
        // OpenSearch.Client QueryContainer
        var clauses = new List<QueryContainer>();

        // Add query clause (MatchQuery from OpenSearch.Client) to clauses
        if (request.DispatchId.HasValue)
            clauses.Add(new MatchQuery
            {
                // Infer.Field ... come from OpenSearch.Client
                Field = Infer.Field<DispatchModel, Guid>(d => d.DispatchId),
                Query = request.DispatchId.Value.ToString()
            });

        if (!string.IsNullOrEmpty(request.DispatchStatus))
            clauses.Add(new MatchQuery
            {
                Field = Infer.Field<DispatchModel, string>(d => d.DispatchStatus),
                Query = request.DispatchStatus
            });

        if (request.PriceTotalMin.HasValue || request.PriceTotalMax.HasValue)
            // Add query clause (NumericRangeQuery from OpenSearch.Client) to clauses
            clauses.Add(new NumericRangeQuery
            {
                Field = Infer.Field<DispatchModel, double>(d => d.PriceTotal),
                GreaterThanOrEqualTo = request.PriceTotalMin,
                LessThanOrEqualTo = request.PriceTotalMax
            });

        if (request.PickupDateFrom.HasValue || request.PickupDateTo.HasValue)
            // Add query clause (DateRangeQuery from OpenSearch.Client) to clauses
            clauses.Add(new DateRangeQuery
            {
                Field = Infer.Field<DispatchModel, DateTime>(d => d.PickupDate),
                GreaterThanOrEqualTo = request.PickupDateFrom,
                LessThanOrEqualTo = request.PickupDateTo
            });

        if (request.DropoffDateFrom.HasValue || request.DropoffDateTo.HasValue)
            clauses.Add(new DateRangeQuery
            {
                Field = Infer.Field<DispatchModel, DateTime>(d => d.DropoffDate),
                GreaterThanOrEqualTo = request.DropoffDateFrom,
                LessThanOrEqualTo = request.DropoffDateTo
            });

        if (!string.IsNullOrWhiteSpace(request.VehicleVin))
            // Add query clause (WildcardQuery from OpenSearch.Client) to clauses
            clauses.Add(new WildcardQuery
            {
                Field = "vehicles.vin",
                Value = $"*{request.VehicleVin}*",
                CaseInsensitive = true
            });

        // If no queries (clauses) are provided return all indexes, if yes return only indexes that contain provided queries (clauses)
        QueryContainer query = clauses.Count == 0 ? new MatchAllQuery() : new BoolQuery { Must = clauses };


        return new SearchRequest<DispatchModel>(indexName)
        {
            Query = query
        };
    }
}
