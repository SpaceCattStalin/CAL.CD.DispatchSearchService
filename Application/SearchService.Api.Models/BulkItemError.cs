namespace SearchJobs.Api.Models;

public record class BulkItemError(Guid DispatchId, string ErrorMessage);
