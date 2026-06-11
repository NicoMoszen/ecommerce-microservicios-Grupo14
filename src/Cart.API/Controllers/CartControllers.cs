using Cart.API.DTOs;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>Obtener carrito del usuario.</summary>
    /// <response code="200">Carrito encontrado.</response>
    /// <response code="404">Carrito no encontrado.</response>
    /// <response code="500">Error interno.</response>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCart(Guid userId)
    {
        var cart = await _cartService.GetCartAsync(userId);
        return Ok(cart);
    }

    /// <summary>Agregar producto al carrito.</summary>
    /// <response code="200">Producto agregado correctamente.</response>
    /// <response code="400">Cantidad inválida.</response>
    /// <response code="404">Producto no encontrado.</response>
    /// <response code="422">Stock insuficiente.</response>
    /// <response code="500">Error interno.</response>
    [HttpPost("{userId}/items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddItem(Guid userId, [FromBody] AddCartItemRequest request)
    {
        var cart = await _cartService.AddItemAsync(userId, request);
        return Ok(cart);
    }

    /// <summary>Actualizar cantidad de un item del carrito.</summary>
    /// <response code="200">Cantidad actualizada correctamente.</response>
    /// <response code="400">Cantidad inválida.</response>
    /// <response code="404">Carrito o producto no encontrado.</response>
    /// <response code="422">Stock insuficiente.</response>
    /// <response code="500">Error interno.</response>
    [HttpPut("{userId}/items/{productId}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateItem(Guid userId, Guid productId, [FromBody] UpdateCartItemRequest request)
    {
        var cart = await _cartService.UpdateItemAsync(userId, productId, request);
        return Ok(cart);
    }

    /// <summary>Quitar un producto del carrito.</summary>
    /// <response code="204">Producto eliminado correctamente.</response>
    /// <response code="404">Carrito o producto no encontrado.</response>
    /// <response code="500">Error interno.</response>
    [HttpDelete("{userId}/items/{productId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RemoveItem(Guid userId, Guid productId)
    {
        await _cartService.RemoveItemAsync(userId, productId);
        return NoContent();
    }

    /// <summary>Vaciar carrito completo.</summary>
    /// <response code="204">Carrito vaciado correctamente.</response>
    /// <response code="404">Carrito no encontrado.</response>
    /// <response code="500">Error interno.</response>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ClearCart(Guid userId)
    {
        await _cartService.ClearCartAsync(userId);
        return NoContent();
    }
}   