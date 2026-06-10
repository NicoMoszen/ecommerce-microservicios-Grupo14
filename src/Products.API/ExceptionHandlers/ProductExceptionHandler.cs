using Microsoft.AspNetCore.Diagnostics;
using Products.API.Exceptions;

namespace Products.API.ExceptionHandlers
{
    public class ProductExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not ProductException ex)
            {
                return false;
            }

            context.Response.StatusCode = ex.StatusCode;

            await context.Response.WriteAsJsonAsync(new
            {
                status = ex.StatusCode,
                errorCode = ex.ErrorCode,
                errorMessage = ex.Message
            }, cancellationToken);

            return true;
        }
    }
}