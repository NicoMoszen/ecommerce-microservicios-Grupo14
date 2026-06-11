using Notifications.API.Data;
using Notifications.API.DTOs;
using Notifications.API.Exceptions;
using Notifications.API.Models;

namespace Notifications.API.Services
{
    public class NotificationService
    {
        private readonly NotificationRepository _repository;
        private readonly IHttpClientFactory _httpClientFactory;

        public NotificationService(NotificationRepository repository, IHttpClientFactory httpClientFactory)
        {
            _repository = repository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<NotificationResponse> EnviarAsync(SendNotificationRequest request)
        {
            ValidarRequest(request);

            var client = _httpClientFactory.CreateClient("UsersAPI");
            var response = await client.GetAsync($"/api/users/{request.UsuarioId}");
            if (!response.IsSuccessStatusCode)
                throw new UsuarioNoEncontradoException(request.UsuarioId);

            var notification = new Notification
            {
                UsuarioId = request.UsuarioId,
                Mensaje = request.Mensaje,
                Tipo = request.Tipo
            };

            _repository.Crear(notification);

            return new NotificationResponse
            {
                Id = notification.Id,
                UsuarioId = notification.UsuarioId,
                Mensaje = notification.Mensaje,
                Tipo = notification.Tipo,
                Estado = notification.Estado,
                FechaEnvio = notification.FechaEnvio
            };
        }

        public NotificationResponse[] ObtenerPorUsuario(Guid usuarioId)
        {
            var notificaciones = _repository.ObtenerPorUsuario(usuarioId);
            if (notificaciones.Count == 0)
                throw new NotificacionesNoEncontradasException(usuarioId);

            return notificaciones.Select(n => new NotificationResponse
            {
                Id = n.Id,
                UsuarioId = n.UsuarioId,
                Mensaje = n.Mensaje,
                Tipo = n.Tipo,
                Estado = n.Estado,
                FechaEnvio = n.FechaEnvio
            }).ToArray();
        }

        private void ValidarRequest(SendNotificationRequest request)
        {
            List<string> errores = new List<string>();

            if (request.UsuarioId == Guid.Empty)
                errores.Add("El usuarioId es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Mensaje))
                errores.Add("El mensaje es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Tipo))
                errores.Add("El tipo es obligatorio.");
            else if (request.Tipo != "Email" && request.Tipo != "SMS" && request.Tipo != "Push")
                errores.Add("El tipo debe ser Email, SMS o Push.");

            if (errores.Count > 0)
                throw new DatosInvalidosException(string.Join(" ", errores));
        }
    }
}
