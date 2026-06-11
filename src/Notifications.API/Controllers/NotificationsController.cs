using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Services;

namespace Notifications.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Produces("application/json")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Registrar y simular envío de notificación
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType(typeof(NotificationResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Enviar([FromBody] SendNotificationRequest request)
        {
            var resultado = await _notificationService.EnviarAsync(request);
            return StatusCode(201, resultado);
        }

        /// <summary>
        /// Listar notificaciones de un usuario
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(NotificationResponse[]), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult ObtenerPorUsuario(Guid userId)
        {
            var resultado = _notificationService.ObtenerPorUsuario(userId);
            return Ok(resultado);
        }
    }
}