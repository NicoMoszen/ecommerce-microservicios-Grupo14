using Dapper;
using Microsoft.Data.Sqlite;

namespace Notifications.API.Data
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=notifications.db";
        }

        public void Initialize()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS notificaciones (
                    id          TEXT    PRIMARY KEY,
                    usuario_id  TEXT    NOT NULL,
                    mensaje     TEXT    NOT NULL,
                    tipo        TEXT    NOT NULL,
                    estado      TEXT    NOT NULL DEFAULT 'Enviada',
                    fecha_envio TEXT    NOT NULL
                );
            ");
        }
    }
}
