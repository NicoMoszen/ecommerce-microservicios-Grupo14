namespace Cart.API.Exceptions;

public class CartProductNotFoundException : Exception
{
	public CartProductNotFoundException() : base("Producto no encontrado.") { }
}
