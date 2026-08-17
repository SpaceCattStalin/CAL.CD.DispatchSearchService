namespace SearchService.Api.Infrastructure.OpenSearch;

public class OpenSearchOptions
{
    public const string SectionName = "OpenSearch";

    public string Uri { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string IndexName { get; set; } = string.Empty;
}