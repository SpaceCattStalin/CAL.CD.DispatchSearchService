using System.Diagnostics.Eventing.Reader;

namespace SearchService.Api.Core.Interfaces;

public class DispatchIndexResult(bool success, string? Id, string? Error)
{
    public bool success { get; } = success;
    public string? Id { get; } = Id;
    public string? Error { get; } = Error;

    public override string ToString() => $"DispatchIndexResult {{ success = {success}, Id = {Id}, Error = {Error} }}";
}

public class DispatchDeleteResult(bool success, bool NotFound, string? Error)
{
    public bool success { get; } = success;
    public bool NotFound { get; } = NotFound;
    public string? Error { get; } = Error;

    public override string ToString() => $"DispatchDeleteResult {{ success = {success}, NotFound = {NotFound}, Error = {Error} }}";
}

public class DispatchUpdateResult(bool success, string? Error)
{
    public bool success { get; } = success;
    public string? Error { get; } = Error;

    public override string ToString() => $"DispatchUpdateResult {{ success = {success}, Error = {Error} }}";
}

public class BulkUpdateResult(int SuccessCount, List<BulkItemError> Errors)
{
    public int SuccessCount { get; } = SuccessCount;
    public List<BulkItemError> Errors { get; } = Errors;

    public override string ToString() => $"BulkUpdateResult {{ SuccessCount = {SuccessCount}, Errors = {Errors} }}";
}
