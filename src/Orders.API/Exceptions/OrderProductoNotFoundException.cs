namespace Orders.API.Exceptions;

public class OrderProductoNotFoundException : Exception
{
    public OrderProductoNotFoundException(string message = "Producto no encontrado al crear la orden.")
        : base(message) { }
}