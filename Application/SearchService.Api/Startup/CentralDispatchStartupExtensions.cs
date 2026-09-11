using Microsoft.Extensions.Options;
using SearchService.Api.Core.Interfaces;
using SearchService.Api.Infrastructure.CentralDispatch;

namespace SearchService.Api.Startup;

public static class CentralDispatchStartupExtensions
{
    public static IServiceCollection AddCentralDispatchClient(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddHttpClient<ICentralDispatchClient, CentralDispatchClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AppSettings>>().Value.CentralDispatch;
            client.BaseAddress = new Uri(options.BaseUrl);
        });

        return services;
    }
}
