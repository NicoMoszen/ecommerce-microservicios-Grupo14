using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class OrderDatosInvalidosExceptionHandler : IExceptionHandler
{
    private readonly ILogger<OrderDatosInvalidosExceptionHandler> _logger;

    public OrderDatosInvalidosExceptionHandler(ILogger<OrderDatosInvalidosExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not OrderDatosInvalidosException) return false;

        // Error de negocio -> Warning, con errorCode (requisito de la consigna).
        _logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}",
            "ORD-002", context.Request.Path, exception.Message);

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
                ["errorCode"] = "ORD-002",
                ["errorMessage"] = exception.Message,
                ["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName]
            }
        }, ct);

        return true;
    }
}