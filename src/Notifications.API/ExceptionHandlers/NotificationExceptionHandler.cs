using Microsoft.AspNetCore.Diagnostics;
using Notifications.API.Exceptions;

namespace Notifications.API.ExceptionHandlers
{
    public class NotificationExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (status, title, errorCode, errorMessage) = exception switch
            {
                UsuarioNoEncontradoException ex => (404, "Not Found", ex.ErrorCode, ex.Message),
                DatosInvalidosException ex => (400, "Bad Request", ex.ErrorCode, ex.Message),
                NotificacionesNoEncontradasException ex => (404, "Not Found", ex.ErrorCode, ex.Message),
                ErrorInternoException ex => (500, "Internal Server Error", ex.ErrorCode, ex.Message),
                _ => (500, "Internal Server Error", "NTF-004", "Error interno al procesar la notificación.")
            };

            context.Response.StatusCode = status;

            var problema = new
            {
                type = $"https://httpstatuses.com/{status}",
                title = title,
                status = status,
                detail = errorMessage,
                instance = context.Request.Path.Value,
                errorCode = errorCode,
                errorMessage = errorMessage
            };

            await context.Response.WriteAsJsonAsync(problema, cancellationToken);
            return true;
        }
    }
}
