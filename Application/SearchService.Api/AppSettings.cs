using System.ComponentModel.DataAnnotations;

namespace SearchService.Api;

public class AppSettings
{
    [Required]
    public required OpenSearchSettings OpenSearch { get; init; }
}

public class OpenSearchSettings
{
    [Required]
    public required string Uri { get; init; }

    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }

    [Required]
    public required string IndexName { get; init; }
}
