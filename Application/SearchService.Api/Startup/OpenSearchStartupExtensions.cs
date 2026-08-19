using Microsoft.Extensions.Options;
using OpenSearch.Client;
using SearchService.Api.Infrastructure.OpenSearch;

namespace SearchService.Api.Startup;

public static class OpenSearchStartupExtensions
{
    public static IServiceCollection AddOpenSearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenSearchOptions>(configuration.GetSection(OpenSearchOptions.SectionName));

        services.AddSingleton<IOpenSearchClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OpenSearchOptions>>().Value;

            var connectionSettings = new ConnectionSettings(new Uri(options.Uri))
                .DefaultIndex(options.IndexName)
                .ServerCertificateValidationCallback((o, cert, chain, errors) => true)
                .BasicAuthentication(options.Username, options.Password);

            return new OpenSearchClient(connectionSettings);
        });

        services.AddScoped<DispatchIndexInitializer>();

        return services;
    }
}
