using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Products.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Error inesperado al procesar {Path}", context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = 500,
                Detail = "Ocurrió un error inesperado.",
                Instance = context.Request.Path,
                Extensions = new Dictionary<string, object?>
                {
                    ["errorCode"] = "PRD-005",
                    ["errorMessage"] = "Error interno al procesar el producto.",
                    ["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName]
                }
            }, cancellationToken);

            return true;
        }
    }
}