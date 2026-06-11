using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Cart.API.Exceptions;

namespace Cart.API.ExceptionHandlers;

public class CartNotFoundExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is not CartNotFoundException) return false;

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
                ["errorMessage"] = exception.Message
            }
        }, ct);

        return true;
    }
}