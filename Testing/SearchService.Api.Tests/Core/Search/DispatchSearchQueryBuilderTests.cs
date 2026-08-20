using OpenSearch.Client;
using SearchService.Api.Core.Search;
using SearchService.Api.Models.Search;

namespace SearchService.Api.Tests.Core.Search;

public class DispatchSearchQueryBuilderTests
{
    private readonly DispatchSearchQueryBuilder _builder = new();

    private static IQueryContainer AsContainer(QueryContainer? query)
    {
        Assert.NotNull(query);
        return query;
    }

    /// <summary>
    /// The builder always wraps non-empty filter sets in bool.must, even a single clause
    /// (valid, working query DSL - confirmed against a live OpenSearch instance). Tests for an
    /// individual filter need to unwrap through Bool.Must to reach the actual clause.
    /// </summary>
    private static IQueryContainer GetSingleClause(QueryContainer? query)
    {
        var container = AsContainer(query);
        if (container.Bool is null)
            return container;

        var must = container.Bool.Must!.ToList();
        Assert.Single(must);
        return must[0];
    }

    [Fact]
    public void Build_NoFilters_ReturnsMatchAllQuery()
    {
        var request = new DispatchSearchRequestModel();

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        Assert.NotNull(AsContainer(result.Query).MatchAll);
    }

    [Fact]
    public void Build_DispatchIdSet_ReturnsMatchQueryOnDispatchId()
    {
        var dispatchId = Guid.NewGuid();
        var request = new DispatchSearchRequestModel { DispatchId = dispatchId };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        var match = GetSingleClause(result.Query).Match;
        Assert.NotNull(match);
        Assert.Equal("d => d.DispatchId", match!.Field!.Expression!.ToString());
        Assert.Equal(dispatchId.ToString(), match.Query);
    }

    [Fact]
    public void Build_DispatchStatusSet_ReturnsMatchQueryOnDispatchStatus()
    {
        var request = new DispatchSearchRequestModel { DispatchStatus = "Delivered" };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        var match = GetSingleClause(result.Query).Match;
        Assert.NotNull(match);
        Assert.Equal("d => d.DispatchStatus", match!.Field!.Expression!.ToString());
        Assert.Equal("Delivered", match.Query);
    }

    [Fact]
    public void Build_PriceTotalRange_ReturnsNumericRangeQueryWithBothBounds()
    {
        var request = new DispatchSearchRequestModel { PriceTotalMin = 100, PriceTotalMax = 500 };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        var range = GetSingleClause(result.Query).Range;
        Assert.NotNull(range);
        var numeric = Assert.IsAssignableFrom<INumericRangeQuery>(range);
        var field = Assert.IsAssignableFrom<IFieldNameQuery>(range);
        Assert.Equal("d => d.PriceTotal", field.Field!.Expression!.ToString());
        Assert.Equal(100, numeric.GreaterThanOrEqualTo);
        Assert.Equal(500, numeric.LessThanOrEqualTo);
    }

    [Fact]
    public void Build_PriceTotalMinOnly_LeavesUpperBoundNull()
    {
        var request = new DispatchSearchRequestModel { PriceTotalMin = 100 };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        var numeric = Assert.IsAssignableFrom<INumericRangeQuery>(GetSingleClause(result.Query).Range);
        Assert.Equal(100, numeric.GreaterThanOrEqualTo);
        Assert.Null(numeric.LessThanOrEqualTo);
    }

    [Fact]
    public void Build_PickupDateRange_ReturnsDateRangeQueryOnPickupDate()
    {
        var from = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 7, 31, 0, 0, 0, DateTimeKind.Utc);
        var request = new DispatchSearchRequestModel { PickupDateFrom = from, PickupDateTo = to };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        var range = GetSingleClause(result.Query).Range;
        var date = Assert.IsAssignableFrom<IDateRangeQuery>(range);
        var field = Assert.IsAssignableFrom<IFieldNameQuery>(range);
        Assert.Equal("d => d.PickupDate", field.Field!.Expression!.ToString());
        Assert.Equal(((DateMath)from).ToString(), date.GreaterThanOrEqualTo!.ToString());
        Assert.Equal(((DateMath)to).ToString(), date.LessThanOrEqualTo!.ToString());
    }

    [Fact]
    public void Build_DropoffDateRange_ReturnsDateRangeQueryOnDropoffDate()
    {
        var from = new DateTime(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = new DispatchSearchRequestModel { DropoffDateFrom = from };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        var range = GetSingleClause(result.Query).Range;
        var date = Assert.IsAssignableFrom<IDateRangeQuery>(range);
        var field = Assert.IsAssignableFrom<IFieldNameQuery>(range);
        Assert.Equal("d => d.DropoffDate", field.Field!.Expression!.ToString());
        Assert.Equal(((DateMath)from).ToString(), date.GreaterThanOrEqualTo!.ToString());
        Assert.Null(date.LessThanOrEqualTo);
    }

    [Fact]
    public void Build_VehicleVinSet_ReturnsCaseInsensitiveWildcardQuery()
    {
        var request = new DispatchSearchRequestModel { VehicleVin = "1HGCM82" };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        var wildcard = GetSingleClause(result.Query).Wildcard;
        Assert.NotNull(wildcard);
        Assert.Equal("vehicles.vin", wildcard!.Field!.Name);
        Assert.Equal("*1HGCM82*", wildcard.Value);
        Assert.True(wildcard.CaseInsensitive);
    }

    [Fact]
    public void Build_MultipleFilters_CombinesIntoBoolMustWithAllClauses()
    {
        var request = new DispatchSearchRequestModel
        {
            DispatchStatus = "Delivered",
            PriceTotalMin = 100,
            VehicleVin = "1HGCM82"
        };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        var boolQuery = AsContainer(result.Query).Bool;
        Assert.NotNull(boolQuery);
        Assert.Equal(3, boolQuery!.Must!.Count());
    }

    [Theory]
    [InlineData(1, 20, 0, 20)]
    [InlineData(2, 20, 20, 20)]
    [InlineData(3, 10, 20, 10)]
    public void Build_Paging_ComputesFromAndSize(int page, int pageSize, int expectedFrom, int expectedSize)
    {
        var request = new DispatchSearchRequestModel { Page = page, PageSize = pageSize };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        Assert.Equal(expectedFrom, result.From);
        Assert.Equal(expectedSize, result.Size);
    }

    [Fact]
    public void Build_PageLessThanOne_ClampsFromToZero()
    {
        var request = new DispatchSearchRequestModel { Page = 0, PageSize = 20 };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        Assert.Equal(0, result.From);
    }

    [Fact]
    public void Build_PageSizeAboveMax_ClampsSizeTo100()
    {
        var request = new DispatchSearchRequestModel { Page = 1, PageSize = 1000 };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        Assert.Equal(100, result.Size);
    }

    [Fact]
    public void Build_PageSizeBelowMin_ClampsSizeTo1()
    {
        var request = new DispatchSearchRequestModel { Page = 1, PageSize = 0 };

        var result = _builder.BuildOpenSearchRequest(request, "dispatches");

        Assert.Equal(1, result.Size);
    }
}
