namespace SearchService.Api.Models.Search;

public class DispatchSearchResponseModel
{
    public long Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyCollection<DispatchModel> Items { get; set; } = [];
}
