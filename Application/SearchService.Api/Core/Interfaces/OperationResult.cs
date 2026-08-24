using System.Diagnostics.Eventing.Reader;
using SearchJobs.Api.Models;

namespace SearchService.Api.Core.Interfaces;

public record DispatchIndexResult(bool success, string? Id, string? Error);

public record DispatchDeleteResult(bool success, bool NotFound, string? Error);

public record DispatchUpdateResult(bool success, string? Error);

public record class BulkUpdateResult(int SuccessCount, List<BulkItemError> Errors);
