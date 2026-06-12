using System.Reflection;
using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notifications.API.Data;
using Notifications.API.ExceptionHandlers;
using Notifications.API.HealthChecks;
using Notifications.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppLogging("Notifications.API");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

builder.Services.AddExceptionHandler<NotificationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddCheck<NotificationsSqliteHealthCheck>("sqlite-db", tags: ["ready"]);

builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddSingleton<NotificationRepository>();
builder.Services.AddSingleton<NotificationService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();

// Cliente HTTP a Users API (validar destinatario). La URL sale de
// configuración, no hardcodeada. Propaga el correlationId saliente.
builder.Services.AddHttpClient("UsersAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:UsersApi"]!);
    client.Timeout = TimeSpan.FromSeconds(5);
}).AddHttpMessageHandler<CorrelationIdDelegatingHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().Initialize();
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