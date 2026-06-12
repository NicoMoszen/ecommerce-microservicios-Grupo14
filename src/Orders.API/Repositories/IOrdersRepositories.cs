using Orders.API.Models;

namespace Orders.API.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllAsync(Guid? usuarioId);

    Task<Order?> GetByIdAsync(Guid id);

    Task CreateAsync(Order order);

    Task UpdateStatusAsync(Guid id, string estado, DateTime fechaActualizacion);

    Task<int> CountActiveByProductAsync(Guid productId);
}