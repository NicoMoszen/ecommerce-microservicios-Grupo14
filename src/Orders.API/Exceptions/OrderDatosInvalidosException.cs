namespace Orders.API.Exceptions;

public class OrderDatosInvalidosException : Exception
{
    public OrderDatosInvalidosException(string message = "Los datos de la orden son inválidos.")
        : base(message) { }
}