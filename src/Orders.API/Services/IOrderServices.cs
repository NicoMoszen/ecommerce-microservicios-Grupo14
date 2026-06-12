using Orders.API.DTOs;
using Orders.API.Models;

namespace Orders.API.Services;

public interface IOrderService
{
    Task<List<Order>> GetAllAsync(Guid? usuarioId);

    Task<Order> GetByIdAsync(Guid id);

    Task<Order> CreateAsync(CreateOrderRequest request);

    Task<Order> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request);

    Task<int> CountActiveByProductAsync(Guid productId);
}