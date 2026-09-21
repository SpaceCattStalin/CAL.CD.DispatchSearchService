namespace SearchService.Api.Models.CentralDispatch;

public class CentralDispatchCompanyResponse
{
    public Guid CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string CompanyPhone { get; init; } = string.Empty;
    public string CompanyEmail { get; init; } = string.Empty;
}
