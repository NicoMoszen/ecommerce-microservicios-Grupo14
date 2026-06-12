using ECommerce.Shared.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers;

public class OrderNotFoundExceptionHandler : IExceptionHandler
{
	private readonly ILogger<OrderNotFoundExceptionHandler> _logger;

	public OrderNotFoundExceptionHandler(ILogger<OrderNotFoundExceptionHandler> logger)
	{
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
	{
		if (exception is not OrderNotFoundException) return false;

		_logger.LogWarning("{ErrorCode} en {Path}: {Mensaje}",
			"ORD-001", context.Request.Path, exception.Message);

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
				["errorCode"] = "ORD-001",
				["errorMessage"] = exception.Message,
				["correlationId"] = context.Items[CorrelationIdMiddleware.HeaderName]
			}
		}, ct);

		return true;
	}
}