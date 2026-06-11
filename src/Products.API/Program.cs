using ECommerce.Shared.Observability;
using Products.API.Data;
using Products.API.ExceptionHandlers;
using Products.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppLogging("Products.API");

builder.Services.AddOpenApi();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DatabaseInitializer>();

builder.Services.AddSingleton<ProductRepository>();

builder.Services.AddSingleton<ProductService>();

builder.Services.AddExceptionHandler<ProductExceptionHandler>();

builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider
        .GetRequiredService<DatabaseInitializer>()
        .Initialize();
}

app.UseExceptionHandler();

app.UseAppRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapHealthChecks("/health");

app.MapHealthChecks("/health/live");

app.MapHealthChecks("/health/ready");

app.Run();