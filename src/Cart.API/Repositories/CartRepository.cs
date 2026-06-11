using Dapper;
using Microsoft.Data.Sqlite;
using Cart.API.Models;
using CartEntity = Cart.API.Models.Cart;

namespace Cart.API.Repositories;

public class CartRepository : ICartRepository
{
    private readonly string _connectionString;

    public CartRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=cart.db";
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task<CartEntity?> GetByUserIdAsync(Guid userId)
    {
        using var conn = CreateConnection();
        var rows = await conn.QueryAsync<dynamic>("""
            SELECT c.usuario_id, i.producto_id, i.cantidad, c.fecha_actualizacion
            FROM carts c
            LEFT JOIN cart_items i ON c.usuario_id = i.usuario_id
            WHERE c.usuario_id = @UserId
            """, new { UserId = userId.ToString() });

        var list = rows.ToList();
        if (!list.Any()) return null;

        return new CartEntity
        {
            UsuarioId = userId,
            FechaActualizacion = DateTime.Parse((string)list[0].fecha_actualizacion),
            Items = list
                .Where(r => r.producto_id != null)
                .Select(r => new CartItem
                {
                    ProductoId = Guid.Parse((string)r.producto_id),
                    Cantidad = (int)r.cantidad
                }).ToList()
        };
    }

    public async Task SaveAsync(CartEntity cart)
    {
        using var conn = CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        await conn.ExecuteAsync("""
            INSERT INTO carts (usuario_id, fecha_actualizacion)
            VALUES (@UsuarioId, @FechaActualizacion)
            ON CONFLICT(usuario_id) DO UPDATE SET fecha_actualizacion = @FechaActualizacion
            """, new { UsuarioId = cart.UsuarioId.ToString(), cart.FechaActualizacion }, transaction);

        await conn.ExecuteAsync(
            "DELETE FROM cart_items WHERE usuario_id = @UserId",
            new { UserId = cart.UsuarioId.ToString() }, transaction);

        foreach (var item in cart.Items)
        {
            await conn.ExecuteAsync("""
                INSERT INTO cart_items (usuario_id, producto_id, cantidad)
                VALUES (@UsuarioId, @ProductoId, @Cantidad)
                """, new { UsuarioId = cart.UsuarioId.ToString(), ProductoId = item.ProductoId.ToString(), item.Cantidad }, transaction);
        }

        transaction.Commit();
    }

    public async Task DeleteAsync(Guid userId)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync(
            "DELETE FROM carts WHERE usuario_id = @UserId",
            new { UserId = userId.ToString() });
    }
}