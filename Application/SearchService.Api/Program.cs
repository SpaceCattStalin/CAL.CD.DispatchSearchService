using DotNetEnv;
using SearchService.Api.Infrastructure.OpenSearch;
using SearchService.Api.Startup;

Env.Load(Path.Combine(Directory.GetCurrentDirectory(), "../..", ".env"));

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddOpenSearch(builder.Configuration);
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
