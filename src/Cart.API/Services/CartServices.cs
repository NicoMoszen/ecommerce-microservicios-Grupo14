using Cart.API.DTOs;
using Cart.API.Exceptions;
using Cart.API.Models;
using Cart.API.Repositories;
using CartEntity = Cart.API.Models.Cart;

namespace Cart.API.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _repository;

    public CartService(ICartRepository repository)
    {
        _repository = repository;
    }

    public async Task<CartResponse> GetCartAsync(Guid userId)
    {
        var cart = await _repository.GetByUserIdAsync(userId);
        if (cart is null) throw new CartNotFoundException();
        return CartResponse.FromEntity(cart);
    }

    public async Task<CartResponse> AddItemAsync(Guid userId, AddCartItemRequest request)
    {
        if (request.Cantidad <= 0) throw new CartCantidadInvalidaException();

        var cart = await _repository.GetByUserIdAsync(userId) ?? new CartEntity
        {
            UsuarioId = userId,
            FechaActualizacion = DateTime.UtcNow
        };

        var item = cart.Items.FirstOrDefault(i => i.ProductoId == request.ProductoId);
        if (item is null)
            cart.Items.Add(new CartItem { ProductoId = request.ProductoId, Cantidad = request.Cantidad });
        else
            item.Cantidad += request.Cantidad;

        cart.Touch();
        await _repository.SaveAsync(cart);
        return CartResponse.FromEntity(cart);
    }

    public async Task<CartResponse> UpdateItemAsync(Guid userId, Guid productId, UpdateCartItemRequest request)
    {
        if (request.Cantidad <= 0) throw new CartCantidadInvalidaException();

        var cart = await _repository.GetByUserIdAsync(userId);
        if (cart is null) throw new CartNotFoundException();

        var item = cart.Items.FirstOrDefault(i => i.ProductoId == productId);
        if (item is null) throw new CartProductNotFoundException();

        item.Cantidad = request.Cantidad;
        cart.Touch();
        await _repository.SaveAsync(cart);
        return CartResponse.FromEntity(cart);
    }

    public async Task RemoveItemAsync(Guid userId, Guid productId)
    {
        var cart = await _repository.GetByUserIdAsync(userId);
        if (cart is null) throw new CartNotFoundException();

        var item = cart.Items.FirstOrDefault(i => i.ProductoId == productId);
        if (item is null) throw new CartProductNotFoundException();

        cart.Items.Remove(item);
        cart.Touch();
        await _repository.SaveAsync(cart);
    }

    public async Task ClearCartAsync(Guid userId)
    {
        var cart = await _repository.GetByUserIdAsync(userId);
        if (cart is null) throw new CartNotFoundException();

        await _repository.DeleteAsync(userId);
    }
}