using SearchService.Api;
using SearchService.Api.Infrastructure.OpenSearch;
using SearchService.Api.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOptions<AppSettings>()
    .Bind(builder.Configuration)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOpenSearch();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DispatchIndexInitializer>();
    await initializer.EnsureIndexAsync();
}

app.MapControllers();

app.Run();
