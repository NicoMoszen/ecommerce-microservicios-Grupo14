using Dapper;
using Microsoft.Data.Sqlite;

namespace Users.API.Data
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=users.db";
        }

        public void Initialize()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS usuarios (
                    id                TEXT    PRIMARY KEY,
                    nombre            TEXT    NOT NULL,
                    apellido          TEXT    NOT NULL,
                    email             TEXT    NOT NULL UNIQUE,
                    password_hash     TEXT    NOT NULL,
                    activo            INTEGER NOT NULL DEFAULT 1,
                    intentos_fallidos INTEGER NOT NULL DEFAULT 0,
                    fecha_registro    TEXT    NOT NULL
                );
            ");
        }
    }
}