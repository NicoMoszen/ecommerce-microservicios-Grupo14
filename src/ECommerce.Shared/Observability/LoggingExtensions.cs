using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ECommerce.Shared.Observability
{
    public static class LoggingExtensions
    {
        public static void AddAppLogging(this WebApplicationBuilder builder, string applicationName)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Application", applicationName)
                .WriteTo.Console()
                .WriteTo.File(
                    "logs/log-.txt",
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