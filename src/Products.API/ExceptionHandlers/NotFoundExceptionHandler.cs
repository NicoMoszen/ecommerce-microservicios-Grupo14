using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Products.API.Exceptions;

namespace Products.API.ExceptionHandlers
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<NotFoundExceptionHandler> _logger;

        public NotFoundExceptionHandler(ILogger<NotFoundExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not NotFoundException ex)
            {
                return false;
            }

            _logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}",
                ex.ErrorCode, context.Request.Path, ex.Message);

            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "Not Found",
                Status = 404,
                Detail = "El recurso solicitado no fue encontrado.",
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
    }
}