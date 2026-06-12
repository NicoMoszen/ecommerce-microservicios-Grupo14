namespace Cart.API.Clients;

public interface IProductsApiClient
{
    Task<ProductDto?> GetProductAsync(Guid productId);
}
