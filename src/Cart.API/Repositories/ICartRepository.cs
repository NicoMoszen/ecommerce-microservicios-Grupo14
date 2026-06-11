using CartEntity = Cart.API.Models.Cart;

namespace Cart.API.Repositories;

public interface ICartRepository
{
    Task<CartEntity?> GetByUserIdAsync(Guid userId);
    Task SaveAsync(CartEntity cart);
    Task DeleteAsync(Guid userId);
}