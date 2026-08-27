namespace SearchService.Api.Models;

public class DispatchBatchUpdateRequest(List<DispatchModel> Documents)
{
    public List<DispatchModel> Documents { get; } = Documents;
}
