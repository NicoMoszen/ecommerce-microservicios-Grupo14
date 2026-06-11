using CartEntity = Cart.API.Models.Cart;

namespace Cart.API.DTOs;

public class CartResponse
{
    public Guid UsuarioId { get; set; }

    public List<CartItemResponse> Items { get; set; } = [];

    public DateTime FechaActualizacion { get; set; }

    public static CartResponse FromEntity(CartEntity cart) => new()
    {
        UsuarioId = cart.UsuarioId,
        FechaActualizacion = cart.FechaActualizacion,
        Items = cart.Items
            .Select(i => new CartItemResponse { ProductoId = i.ProductoId, Cantidad = i.Cantidad })
            .ToList(),
    };
}
