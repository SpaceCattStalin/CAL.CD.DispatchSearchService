namespace SearchJobs.Api.Models;

public class BulkItemError(Guid DispatchId, string ErrorMessage)
{
    public Guid DispatchId { get; } = DispatchId;
    public string ErrorMessage { get; } = ErrorMessage;
}
