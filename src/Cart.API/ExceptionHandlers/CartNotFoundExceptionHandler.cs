using Cart.API.Exceptions;
using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.ExceptionHandlers;

public class CartNotFoundExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CartNotFoundExceptionHandler> _logger;

    public CartNotFoundExceptionHandler(ILogger<CartNotFoundExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not CartNotFoundException) return false;

        _logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}",
            "CRT-001", context.Request.Path, exception.Message);

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
                ["errorCode"] = "CRT-001",
                ["errorMessage"] = exception.Message,
                ["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName]
            }
        }, ct);

        return true;
    }
}