using Dapper;
using Microsoft.Data.Sqlite;

namespace Cart.API.Database;

public class DatabaseInitializer
{
	private readonly string _connectionString;

	public DatabaseInitializer(IConfiguration config)
	{
		_connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db";
	}

	public void Initialize()
	{
		using var conn = new SqliteConnection(_connectionString);
		conn.Open();
		conn.Execute("""
            CREATE TABLE IF NOT EXISTS carts (
                usuario_id TEXT PRIMARY KEY,
                fecha_actualizacion TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS cart_items (
                usuario_id TEXT NOT NULL,
                producto_id TEXT NOT NULL,
                cantidad INTEGER NOT NULL,
                PRIMARY KEY (usuario_id, producto_id),
                FOREIGN KEY (usuario_id) REFERENCES carts(usuario_id) ON DELETE CASCADE
            );
            """);
	}
}