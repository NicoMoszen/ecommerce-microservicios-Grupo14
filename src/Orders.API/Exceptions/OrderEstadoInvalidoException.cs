namespace Orders.API.Exceptions;

// ORD-006 -> 409 (transición de estado no válida)
// Se lanza cuando el PUT /status pide una transición fuera de la
// máquina de estados definida en Models/Order.cs (ej: Entregada y
// Cancelada son terminales, una Enviada no puede cancelarse, etc).
// Ejemplo de mensaje: "Una orden en estado 'Entregada' no puede volver a 'Pendiente'."
public class OrderEstadoInvalidoException : Exception
{
    public OrderEstadoInvalidoException(string message = "El estado de la orden no puede ser modificado.")
        : base(message) { }
}