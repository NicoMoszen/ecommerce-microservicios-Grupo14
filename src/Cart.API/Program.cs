using System.Reflection;
using Cart.API.Clients;
using Cart.API.Database;
using Cart.API.ExceptionHandlers;
using Cart.API.HealthChecks;
using Cart.API.Repositories;
using Cart.API.Services;
using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppLogging("Cart.API");

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
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

builder.Services.AddHttpClient<IProductsApiClient, ProductsApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ProductsApi"]!);
    client.Timeout = TimeSpan.FromSeconds(5);
}).AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

builder.Services.AddHealthChecks()
    .AddCheck<CartSqliteHealthCheck>("sqlite-db", tags: ["ready"]);

builder.Services.AddExceptionHandler<CartNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<CartProductNotFoundExceptionHandler>();
builder.Services.AddExceptionHandler<CartStockInsuficienteExceptionHandler>();
builder.Services.AddExceptionHandler<CartCantidadInvalidaExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var result = new ObjectResult(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = "Los datos enviados son inválidos.",
            Instance = context.HttpContext.Request.Path,
            Extensions = new Dictionary<string, object?>
            {
                ["errorCode"] = "CRT-004",
                ["errorMessage"] = "Los datos enviados tienen un formato inválido.",
                ["correlationId"] = context.HttpContext.Items[CorrelationIdMiddleware.HeaderName]
            }
        })
        {
            StatusCode = 400
        };

        return result;
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().Initialize();

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