namespace Cart.API.Exceptions;

public class CartNotFoundException : Exception
{
    public CartNotFoundException() : base("Carrito no encontrado.") { }
}