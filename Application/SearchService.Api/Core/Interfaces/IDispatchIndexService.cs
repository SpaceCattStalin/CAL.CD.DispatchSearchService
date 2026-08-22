using SearchService.Api.Models;

namespace SearchService.Api.Core.Interfaces;

public interface IDispatchIndexService
{
    Task<DispatchIndexResult> IndexAsync(DispatchModel dispatch);

    Task<DispatchDeleteResult> DeleteAsync(Guid dispatchId);

    Task<DispatchUpdateResult> UpdateAsync(DispatchModel dispatch);
}


