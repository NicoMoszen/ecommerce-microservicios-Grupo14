using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cart.API.Exceptions;

namespace Cart.API.ExceptionHandlers;

public class CartCantidadInvalidaExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not CartCantidadInvalidaException) return false;

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
                ["errorMessage"] = exception.Message
            }
        }, ct);

        return true;
    }
}