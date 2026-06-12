using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Users.API.HealthChecks;

public class UsersSqliteHealthCheck : IHealthCheck
{
	private readonly string _connectionString;

	public UsersSqliteHealthCheck(IConfiguration config)
	{
		_connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=users.db";
	}

	public async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context, CancellationToken cancellationToken = default)
	{
		try
		{
			using var conn = new SqliteConnection(_connectionString);
			await conn.OpenAsync(cancellationToken);
			await conn.ExecuteScalarAsync<int>("SELECT 1");
			return HealthCheckResult.Healthy("Conexión a SQLite OK.");
		}
		catch (Exception ex)
		{
			return HealthCheckResult.Unhealthy("No se pudo conectar a SQLite.", ex);
		}
	}
}