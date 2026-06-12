using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Products.API.Exceptions;

namespace Products.API.ExceptionHandlers
{
    public class ProductExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ProductExceptionHandler> _logger;

        public ProductExceptionHandler(ILogger<ProductExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not ProductException ex)
            {
                return false;
            }

            _logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}",
                ex.ErrorCode, context.Request.Path, ex.Message);

            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Type = $"https://httpstatuses.com/{ex.StatusCode}",
                Title = ReasonPhrase(ex.StatusCode),
                Status = ex.StatusCode,
                Detail = ex.Message,
                Instance = context.Request.Path,
                Extensions = new Dictionary<string, object?>
                {
                    ["errorCode"] = ex.ErrorCode,
                    ["errorMessage"] = ex.Message,
                    ["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName]
                }
            }, cancellationToken);

            return true;
        }

        private static string ReasonPhrase(int status) => status switch
        {
            400 => "Bad Request",
            409 => "Conflict",
            422 => "Unprocessable Entity",
            _ => "Error"
        };
    }
}