using System.Reflection;
using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Products.API.Clients;
using Products.API.Data;
using Products.API.ExceptionHandlers;
using Products.API.HealthChecks;
using Products.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppLogging("Products.API");

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<ProductRepository>();
builder.Services.AddSingleton<ProductService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

// Cliente HTTP a Orders API (PRD-004: validar órdenes activas antes de borrar).
builder.Services.AddHttpClient<IOrdersApiClient, OrdersApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:OrdersApi"]!);
    client.Timeout = TimeSpan.FromSeconds(5);
}).AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

builder.Services.AddExceptionHandler<ProductExceptionHandler>();
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddCheck<ProductsSqliteHealthCheck>("sqlite-db", tags: ["ready"]);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider
        .GetRequiredService<DatabaseInitializer>()
        .Initialize();
}

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseExceptionHandler();

app.UseAppRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions { ResponseWriter = WriteHealthResponse });
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
});
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = WriteHealthResponse
});

app.Run();

static Task WriteHealthResponse(HttpContext context, Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport report)
{
    context.Response.ContentType = "application/json";
    return context.Response.WriteAsJsonAsync(new
    {
        estado = report.Status.ToString(),
        duracionMs = report.TotalDuration.TotalMilliseconds,
        checks = report.Entries.Select(e => new
        {
            nombre = e.Key,
            estado = e.Value.Status.ToString(),
            descripcion = e.Value.Description
        })
    });
}