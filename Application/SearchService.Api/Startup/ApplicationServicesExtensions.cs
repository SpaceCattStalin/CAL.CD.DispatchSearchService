using FluentValidation;
using SearchService.Api;
using SearchService.Api.Core.Interfaces;
using SearchService.Api.Core.Search;
using SearchService.Api.Models;
using SearchService.Api.Models.Search;
using SearchService.Api.Presentation.Validation;

namespace SearchService.Api.Startup;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDispatchSearchQueryBuilder, DispatchSearchQueryBuilder>();
        services.AddScoped<IDispatchIndexService, DispatchIndexService>();
        services.AddScoped<IDispatchSearchService, DispatchSearchService>();

        services.AddScoped<IValidator<DispatchModel>, DispatchModelValidator>();
        services.AddScoped<IValidator<DispatchSearchRequestModel>, DispatchSearchRequestModelValidator>();

        return services;
    }
}
