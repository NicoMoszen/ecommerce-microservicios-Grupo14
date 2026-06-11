namespace Cart.API.Exceptions;

public class CartStockInsuficienteException : Exception
{
    public CartStockInsuficienteException(string nombreProducto, int disponible, int solicitado)
        : base($"Stock insuficiente para '{nombreProducto}'. Disponible: {disponible}, solicitado: {solicitado}.") { }
}