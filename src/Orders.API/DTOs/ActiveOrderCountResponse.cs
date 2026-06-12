namespace Orders.API.DTOs;

public class ActiveOrdersCountResponse
{
    public Guid ProductoId { get; set; }

    public int OrdenesActivas { get; set; }
}