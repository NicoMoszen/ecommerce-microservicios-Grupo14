using Dapper;
using Microsoft.Data.Sqlite;
using Users.API.Models;

namespace Users.API.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Data Source=users.db";
        }

        private SqliteConnection CreateConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        public User? BuscarPorEmail(string email)
        {
            using var connection = CreateConnection();
            var result = connection.QueryFirstOrDefault<dynamic>(
                "SELECT id, nombre, apellido, email, password_hash, activo, intentos_fallidos, fecha_registro FROM usuarios WHERE email = @Email",
                new { Email = email });

            if (result == null) return null;

            return new User
            {
                Id = Guid.Parse((string)result.id),
                Nombre = (string)result.nombre,
                Apellido = (string)result.apellido,
                Email = (string)result.email,
                PasswordHash = (string)result.password_hash,
                Activo = (long)result.activo == 1,
                IntentosFallidos = (int)(long)result.intentos_fallidos,
                FechaRegistro = DateTime.Parse((string)result.fecha_registro)
            };
        }

        public User? BuscarPorId(Guid id)
        {
            using var connection = CreateConnection();
            var result = connection.QueryFirstOrDefault<dynamic>(
                "SELECT id, nombre, apellido, email, password_hash, activo, intentos_fallidos, fecha_registro FROM usuarios WHERE id = @Id",
                new { Id = id.ToString() });

            if (result == null) return null;

            return new User
            {
                Id = Guid.Parse((string)result.id),
                Nombre = (string)result.nombre,
                Apellido = (string)result.apellido,
                Email = (string)result.email,
                PasswordHash = (string)result.password_hash,
                Activo = (long)result.activo == 1,
                IntentosFallidos = (int)(long)result.intentos_fallidos,
                FechaRegistro = DateTime.Parse((string)result.fecha_registro)
            };
        }

        public User Crear(User user)
        {
            using var connection = CreateConnection();
            connection.Execute(
                "INSERT INTO usuarios (id, nombre, apellido, email, password_hash, activo, intentos_fallidos, fecha_registro) VALUES (@Id, @Nombre, @Apellido, @Email, @PasswordHash, @Activo, @IntentosFallidos, @FechaRegistro)",
                new
                {
                    Id = user.Id.ToString(),
                    user.Nombre,
                    user.Apellido,
                    user.Email,
                    user.PasswordHash,
                    Activo = user.Activo ? 1 : 0,
                    user.IntentosFallidos,
                    FechaRegistro = user.FechaRegistro.ToString("o")
                });
            return user;
        }

        public void Actualizar(User user)
        {
            using var connection = CreateConnection();
            connection.Execute(
                "UPDATE usuarios SET activo = @Activo, intentos_fallidos = @IntentosFallidos WHERE id = @Id",
                new
                {
                    Id = user.Id.ToString(),
                    Activo = user.Activo ? 1 : 0,
                    user.IntentosFallidos
                });
        }
    }
}