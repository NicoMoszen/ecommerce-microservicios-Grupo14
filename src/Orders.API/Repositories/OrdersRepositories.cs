using Dapper;
using Microsoft.Data.Sqlite;
using Orders.API.Models;

namespace Orders.API.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public OrderRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db";
    }

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task<List<Order>> GetAllAsync(Guid? usuarioId)
    {
        using var conn = CreateConnection();
        var rows = await conn.QueryAsync<dynamic>("""
            SELECT o.id, o.usuario_id, o.total, o.estado, o.fecha_creacion, o.fecha_actualizacion,
                   i.producto_id, i.cantidad, i.precio_unitario
            FROM orders o
            LEFT JOIN order_items i ON o.id = i.order_id
            WHERE (@UsuarioId IS NULL OR o.usuario_id = @UsuarioId)
            ORDER BY o.fecha_creacion DESC
            """, new { UsuarioId = usuarioId?.ToString() });

        return rows
            .GroupBy(r => (string)r.id)
            .Select(g => MapOrder(g.ToList()))
            .ToList();
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        using var conn = CreateConnection();
        var rows = await conn.QueryAsync<dynamic>("""
            SELECT o.id, o.usuario_id, o.total, o.estado, o.fecha_creacion, o.fecha_actualizacion,
                   i.producto_id, i.cantidad, i.precio_unitario
            FROM orders o
            LEFT JOIN order_items i ON o.id = i.order_id
            WHERE o.id = @Id
            """, new { Id = id.ToString() });

        var list = rows.ToList();
        if (!list.Any()) return null;

        return MapOrder(list);
    }

    public async Task CreateAsync(Order order)
    {
        using var conn = CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();

        await conn.ExecuteAsync("""
            INSERT INTO orders (id, usuario_id, total, estado, fecha_creacion, fecha_actualizacion)
            VALUES (@Id, @UsuarioId, @Total, @Estado, @FechaCreacion, @FechaActualizacion)
            """, new
        {
            Id = order.Id.ToString(),
            UsuarioId = order.UsuarioId.ToString(),
            order.Total,
            order.Estado,
            order.FechaCreacion,
            order.FechaActualizacion
        }, transaction);

        foreach (var item in order.Items)
        {
            await conn.ExecuteAsync("""
                INSERT INTO order_items (order_id, producto_id, cantidad, precio_unitario)
                VALUES (@OrderId, @ProductoId, @Cantidad, @PrecioUnitario)
                """, new
            {
                OrderId = order.Id.ToString(),
                ProductoId = item.ProductoId.ToString(),
                item.Cantidad,
                item.PrecioUnitario
            }, transaction);
        }

        transaction.Commit();
    }

    public async Task UpdateStatusAsync(Guid id, string estado, DateTime fechaActualizacion)
    {
        using var conn = CreateConnection();
        await conn.ExecuteAsync("""
            UPDATE orders
            SET estado = @Estado, fecha_actualizacion = @FechaActualizacion
            WHERE id = @Id
            """, new { Id = id.ToString(), Estado = estado, FechaActualizacion = fechaActualizacion });
    }

    public async Task<int> CountActiveByProductAsync(Guid productId)
    {
        using var conn = CreateConnection();

        return await conn.ExecuteScalarAsync<int>("""
            SELECT COUNT(DISTINCT o.id)
            FROM orders o
            INNER JOIN order_items i ON o.id = i.order_id
            WHERE i.producto_id = @ProductId
              AND o.estado IN ('Pendiente', 'Confirmada')
            """, new { ProductId = productId.ToString() });
    }

    private static Order MapOrder(List<dynamic> rows)
    {
        var first = rows[0];

        return new Order
        {
            Id = Guid.Parse((string)first.id),
            UsuarioId = Guid.Parse((string)first.usuario_id),
            Total = Convert.ToDecimal(first.total),
            Estado = (string)first.estado,
            FechaCreacion = DateTime.Parse((string)first.fecha_creacion),
            FechaActualizacion = first.fecha_actualizacion is null
                ? null
                : DateTime.Parse((string)first.fecha_actualizacion),
            Items = rows
                .Where(r => r.producto_id != null)
                .Select(r => new OrderItem
                {
                    ProductoId = Guid.Parse((string)r.producto_id),
                    Cantidad = (int)r.cantidad,
                    PrecioUnitario = Convert.ToDecimal(r.precio_unitario)
                }).ToList()
        };
    }
}