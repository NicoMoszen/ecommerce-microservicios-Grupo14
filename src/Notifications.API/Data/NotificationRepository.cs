using Dapper;
using Microsoft.Data.Sqlite;
using Notifications.API.Models;

namespace Notifications.API.Data
{
    public class NotificationRepository
    {
        private readonly string _connectionString;

        public NotificationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=notifications.db";
        }

        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public Notification Crear(Notification notification)
        {
            using var connection = CreateConnection();
            connection.Execute(
                "INSERT INTO notificaciones (id, usuario_id, mensaje, tipo, estado, fecha_envio) VALUES (@Id, @UsuarioId, @Mensaje, @Tipo, @Estado, @FechaEnvio)",
                new
                {
                    Id = notification.Id.ToString(),
                    UsuarioId = notification.UsuarioId.ToString(),
                    notification.Mensaje,
                    notification.Tipo,
                    notification.Estado,
                    FechaEnvio = notification.FechaEnvio.ToString("o")
                });
            return notification;
        }

        public List<Notification> ObtenerPorUsuario(Guid usuarioId)
        {
            using var connection = CreateConnection();
            var results = connection.Query<dynamic>(
                "SELECT id, usuario_id, mensaje, tipo, estado, fecha_envio FROM notificaciones WHERE usuario_id = @UsuarioId",
                new { UsuarioId = usuarioId.ToString() });

            return results.Select(r => new Notification
            {
                Id = Guid.Parse((string)r.id),
                UsuarioId = Guid.Parse((string)r.usuario_id),
                Mensaje = (string)r.mensaje,
                Tipo = (string)r.tipo,
                Estado = (string)r.estado,
                FechaEnvio = DateTime.Parse((string)r.fecha_envio)
            }).ToList();
        }
    }
}
