namespace Cart.API.Models;

public class Cart
{
    public Guid UsuarioId { get; set; }

    public List<CartItem> Items { get; set; } = [];

    public DateTime FechaActualizacion { get; set; }

    public void Touch() => FechaActualizacion = DateTime.UtcNow;
}
