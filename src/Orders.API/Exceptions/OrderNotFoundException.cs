namespace Orders.API.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException() : base("Orden no encontrada.") { }
}