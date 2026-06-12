namespace Orders.API.Models;

public class Order
{
    public Guid Id { get; set; }

    public Guid UsuarioId { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    public decimal Total { get; set; }


    // La orden empieca en pendiente y la --> muestra de que estado a que otro estado puede pasar.
    //   Pendiente   -> Confirmada | Cancelada
    //   Confirmada  -> Enviada    | Cancelada
    //   Enviada     -> Entregada
    //   Entregada   -> (terminal)
    //   Cancelada   -> (terminal)

    public string Estado { get; set; } = "Pendiente";

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }
}