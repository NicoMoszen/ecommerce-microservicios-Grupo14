using Microsoft.AspNetCore.Diagnostics;

namespace Products.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(new
            {
                status = 500,
                errorCode = "PRD-005",
                errorMessage = "Error interno al procesar el producto."
            }, cancellationToken);

            return true;
        }
    }
}