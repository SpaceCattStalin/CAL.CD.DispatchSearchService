using System.ComponentModel.DataAnnotations;

namespace SearchService.Api;

public class AppSettings
{
    [Required]
    public required OpenSearchSettings OpenSearch { get; init; }

    [Required]
    public required CentralDispatchSettings CentralDispatch { get; init; }
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

public class CentralDispatchSettings
{
    [Required]
    public required string BaseUrl { get; init; }
}

public class JwtSettings
{
    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Required]
    public required string SigningKey { get; init; }

    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; init; }
}
