using Microsoft.Extensions.Options;
using OpenSearch.Client;
using SearchService.Api.Infrastructure.OpenSearch;

namespace SearchService.Api.Startup;

public static class OpenSearchStartupExtensions
{
    public static IServiceCollection AddOpenSearch(this IServiceCollection services)
    {
        services.AddSingleton<IOpenSearchClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AppSettings>>().Value.OpenSearch;

            var connectionSettings = new ConnectionSettings(new Uri(options.Uri))
                .DefaultIndex(options.IndexName)
                .ServerCertificateValidationCallback((o, cert, chain, errors) => true)
                .BasicAuthentication(options.Username, options.Password)
                .DisableDirectStreaming()
                .OnRequestCompleted(apiCall =>
                {
                    Console.WriteLine($"{apiCall.HttpMethod} {apiCall.Uri}");
                    if (apiCall.RequestBodyInBytes is not null)
                        Console.WriteLine(System.Text.Encoding.UTF8.GetString(apiCall.RequestBodyInBytes));
                });

            return new OpenSearchClient(connectionSettings);
        });

        services.AddScoped<DispatchIndexInitializer>();

        return services;
    }
}
