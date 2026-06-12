using Cart.API.Exceptions;
using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.ExceptionHandlers;

public class CartStockInsuficienteExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CartStockInsuficienteExceptionHandler> _logger;

    public CartStockInsuficienteExceptionHandler(ILogger<CartStockInsuficienteExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not CartStockInsuficienteException) return false;

        _logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}",
            "CRT-003", context.Request.Path, exception.Message);

        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
            Title = "Unprocessable Entity",
            Status = 422,
            Detail = "No se puede procesar la solicitud.",
            Instance = context.Request.Path,
            Extensions = new Dictionary<string, object?>
            {
                ["errorCode"] = "CRT-003",
                ["errorMessage"] = exception.Message,
                ["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName]
            }
        }, ct);

        return true;
    }
}