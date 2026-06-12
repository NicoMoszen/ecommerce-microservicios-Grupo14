namespace Cart.API.Clients;


public class ProductDto
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public decimal Precio { get; set; }

    public int Stock { get; set; }
}