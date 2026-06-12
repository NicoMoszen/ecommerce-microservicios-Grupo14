using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Users.API.Exceptions;

namespace Users.API.ExceptionHandlers
{
    public class UserExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<UserExceptionHandler> _logger;

        public UserExceptionHandler(ILogger<UserExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (status, title, errorCode, errorMessage) = exception switch
            {
                EmailYaRegistradoException ex => (409, "Conflict", ex.ErrorCode, ex.Message),
                DatosInvalidosException ex => (400, "Bad Request", ex.ErrorCode, ex.Message),
                CredencialesInvalidasException ex => (401, "Unauthorized", ex.ErrorCode, ex.Message),
                UsuarioBloqueadoIntentosFallidosException ex => (403, "Forbidden", ex.ErrorCode, ex.Message),
                UsuarioBloqueadoFraudeException ex => (403, "Forbidden", ex.ErrorCode, ex.Message),
                NotFoundException ex => (404, "Not Found", ex.ErrorCode, ex.Message),
                ErrorInternoException ex => (500, "Internal Server Error", ex.ErrorCode, ex.Message),
                _ => (500, "Internal Server Error", "USR-006", "Error interno al procesar el usuario.")
            };


            if (status >= 500)
                _logger.LogError(exception, "{ErrorCode} en {Path}", errorCode, context.Request.Path);
            else
                _logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}", errorCode, context.Request.Path, errorMessage);

            context.Response.StatusCode = status;

            var problema = new ProblemDetails
            {
                Type = $"https://httpstatuses.com/{status}",
                Title = title,
                Status = status,
                Detail = errorMessage,
                Instance = context.Request.Path
            };

            problema.Extensions["errorCode"] = errorCode;
            problema.Extensions["errorMessage"] = errorMessage;
            problema.Extensions["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName];

            await context.Response.WriteAsJsonAsync(problema, cancellationToken);
            return true;
        }
    }
}