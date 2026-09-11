namespace SearchService.Api.Models.CentralDispatch;

public class CentralDispatchDriverResponse
{
    public Guid DriverId { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
