using Cart.API.Exceptions;
using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.ExceptionHandlers;

public class CartCantidadInvalidaExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CartCantidadInvalidaExceptionHandler> _logger;

    public CartCantidadInvalidaExceptionHandler(ILogger<CartCantidadInvalidaExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not CartCantidadInvalidaException) return false;

        _logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}",
            "CRT-004", context.Request.Path, exception.Message);

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Bad Request",
            Status = 400,
            Detail = "Los datos enviados son inválidos.",
            Instance = context.Request.Path,
            Extensions = new Dictionary<string, object?>
            {
                ["errorCode"] = "CRT-004",
                ["errorMessage"] = exception.Message,
                ["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName]
            }
        }, ct);

        return true;
    }
}