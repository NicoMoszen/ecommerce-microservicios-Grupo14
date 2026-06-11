namespace Cart.API.Exceptions;

public class CartCantidadInvalidaException : Exception
{
	public CartCantidadInvalidaException() : base("Cantidad inválida.") { }
}