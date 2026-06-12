namespace Products.API.Clients;

public class ActiveOrdersCountDto
{
    public Guid ProductoId { get; set; }

    public int OrdenesActivas { get; set; }
}