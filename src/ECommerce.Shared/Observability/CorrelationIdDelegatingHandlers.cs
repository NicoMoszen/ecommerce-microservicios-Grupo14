using Microsoft.AspNetCore.Http;

namespace ECommerce.Shared.Observability
{

    public class CorrelationIdDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var correlationId = _httpContextAccessor.HttpContext?
                .Items[CorrelationIdMiddleware.HeaderName] as string;

            if (!string.IsNullOrEmpty(correlationId))
                request.Headers.TryAddWithoutValidation(
                    CorrelationIdMiddleware.HeaderName, correlationId);

            return base.SendAsync(request, cancellationToken);
        }
    }
}