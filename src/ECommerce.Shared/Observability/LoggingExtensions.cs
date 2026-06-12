using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Formatting.Json;

namespace ECommerce.Shared.Observability
{
    public static class LoggingExtensions
    {
        public static void AddAppLogging(this WebApplicationBuilder builder, string applicationName)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()                 
                .Enrich.WithProperty("Application", applicationName)
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{Application}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
                
                .WriteTo.File(
                    formatter: new JsonFormatter(),
                    path: "logs/log-.json",
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();
        }

        public static void UseAppRequestLogging(this WebApplication app)
        {
            app.UseSerilogRequestLogging();
        }
    }
}