namespace Orders.API.Exceptions;

public class OrderUsuarioNotFoundException : Exception
{
    public OrderUsuarioNotFoundException() : base("Usuario no encontrado al crear la orden.") { }
}