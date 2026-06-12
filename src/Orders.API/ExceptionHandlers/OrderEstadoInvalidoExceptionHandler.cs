using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class OrderEstadoInvalidoExceptionHandler : IExceptionHandler
{
    private readonly ILogger<OrderEstadoInvalidoExceptionHandler> _logger;

    public OrderEstadoInvalidoExceptionHandler(ILogger<OrderEstadoInvalidoExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not OrderEstadoInvalidoException) return false;

        _logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}",
            "ORD-006", context.Request.Path, exception.Message);

        context.Response.StatusCode = StatusCodes.Status409Conflict;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.9",
            Title = "Conflict",
            Status = 409,
            Detail = "No se puede modificar el estado.",
            Instance = context.Request.Path,
            Extensions = new Dictionary<string, object?>
            {
                ["errorCode"] = "ORD-006",
                ["errorMessage"] = exception.Message,
                ["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName]
            }
        }, ct);

        return true;
    }
}