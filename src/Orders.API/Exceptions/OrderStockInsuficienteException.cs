namespace Orders.API.Exceptions;

public class OrderStockInsuficienteException : Exception
{
    public OrderStockInsuficienteException(string message = "Stock insuficiente para uno o más productos.")
        : base(message) { }
}